using Grpc.Core;
using Platform.Application.Abstractions.Data;
using Platform.Catalog.API.Application.Features.Products.Mappers;
using Platform.Catalog.API.Domain.Enums;
using Platform.Catalog.API.Infrastructure.Persistence.Models;
using Platform.Catalog.Grpc;
using Platform.Common.Grpc;

namespace Platform.Catalog.API.Presentation.Grpc;

public sealed class CatalogIntegrationService : CatalogIntegration.CatalogIntegrationBase
{
    private readonly IUnitOfWork _unitOfWork;

    public CatalogIntegrationService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public override async Task<GetProductCartSnapshotResponse> GetProductCartSnapshot(
        GetProductCartSnapshotRequest request,
        ServerCallContext context)
    {
        // Catalog sở hữu dữ liệu product, nên service khác chỉ đọc qua
        // endpoint gRPC này thay vì query trực tiếp CatalogDb.
        if (!Guid.TryParse(request.ProductId, out var productId))
            return CatalogIntegrationResponses.Failure("Invalid product id.");

        var product = await _unitOfWork.GetRepository<ProductModel>().FindAsync(
            x => x.Id == productId && x.Status == ProductStatus.Active,
            true,
            context.CancellationToken);

        if (product is null)
            return CatalogIntegrationResponses.Failure("Product not found.");

        return product.ToSuccessResponse();
    }

    public override async Task<AdjustStockResponse> DecreaseStock(
        AdjustStockRequest request,
        ServerCallContext context)
    {
        if (await HasProcessedOperationAsync(request.OperationId, context.CancellationToken))
            return CatalogIntegrationResponses.SuccessAdjustStock();

        // Catalog là nơi giữ stock thật, nên checkout bên Ordering sẽ gọi
        // sang endpoint này để trừ kho thay vì tự cập nhật dữ liệu Catalog.
        foreach (var item in request.Items)
        {
            if (!Guid.TryParse(item.ProductId, out var productId))
                return CatalogIntegrationResponses.FailureAdjustStock("Invalid product id.");

            if (item.Quantity <= 0)
                return CatalogIntegrationResponses.FailureAdjustStock("Quantity must be greater than 0.");

            var productModel = await _unitOfWork.GetRepository<ProductModel>().FindAsync(
                x => x.Id == productId && x.Status == ProductStatus.Active,
                false,
                context.CancellationToken);

            if (productModel is null)
                return CatalogIntegrationResponses.FailureAdjustStock("Product not found.");

            // Luôn đổi persistence model sang domain để áp business rule ReduceStock,
            // tránh chỉnh stock trực tiếp trên model EF.
            var product = productModel.ToDomain();
            var reduceStockResult = product.ReduceStock(item.Quantity);
            if (reduceStockResult.IsFailure)
                return CatalogIntegrationResponses.FailureAdjustStock(reduceStockResult.Error.Message);

            productModel.ApplyDomainState(product);
        }

        await RememberProcessedOperationAsync(request.OperationId, "decrease", context.CancellationToken);
        await _unitOfWork.SaveChangesAsync(context.CancellationToken);

        return CatalogIntegrationResponses.SuccessAdjustStock();
    }

    public override async Task<AdjustStockResponse> RestoreStock(AdjustStockRequest request, ServerCallContext context)
    {
        if (await HasProcessedOperationAsync(request.OperationId, context.CancellationToken))
            return CatalogIntegrationResponses.SuccessAdjustStock();

        // Catalog tự xử lý trả kho khi Ordering cần hoàn tác stock cho các physical product.
        foreach (var item in request.Items)
        {
            if (!Guid.TryParse(item.ProductId, out var productId))
                return CatalogIntegrationResponses.FailureAdjustStock("Invalid product id.");

            var productModel = await _unitOfWork.GetRepository<ProductModel>().FindAsync(
                x => x.Id == productId && x.Status == ProductStatus.Active,
                false,
                context.CancellationToken);

            if (productModel == null)
                return CatalogIntegrationResponses.FailureAdjustStock("Product not found.");

            // Restock vẫn đi qua domain để giữ đúng rule nghiệp vụ, không sửa thẳng model.
            var product = productModel.ToDomain();
            var restockResult = product.Restock(item.Quantity);
            if (restockResult.IsFailure)
                return CatalogIntegrationResponses.FailureAdjustStock(restockResult.Error.Message);

            productModel.ApplyDomainState(product);
        }

        await RememberProcessedOperationAsync(request.OperationId, "restore", context.CancellationToken);
        await _unitOfWork.SaveChangesAsync(context.CancellationToken);

        return CatalogIntegrationResponses.SuccessAdjustStock();
    }

    private async Task<bool> HasProcessedOperationAsync(string operationId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(operationId))
            return false;

        return await _unitOfWork.GetRepository<StockAdjustmentOperationModel>().FindAsync(
            x => x.OperationId == operationId,
            true,
            cancellationToken) is not null;
    }

    private async Task RememberProcessedOperationAsync(
        string operationId,
        string adjustmentType,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(operationId))
            return;

        await _unitOfWork.GetRepository<StockAdjustmentOperationModel>().AddAsync(
            new StockAdjustmentOperationModel
            {
                OperationId = operationId,
                AdjustmentType = adjustmentType
            },
            cancellationToken);
    }

}

