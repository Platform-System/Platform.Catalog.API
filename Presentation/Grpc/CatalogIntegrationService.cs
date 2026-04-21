using Grpc.Core;
using Platform.Application.Abstractions.Data;
using Platform.Catalog.API.Application.Mappers;
using Platform.Catalog.API.Domain.Entities;
using Platform.Catalog.API.Domain.Enums;
using Platform.Catalog.API.Infrastructure.Persistence.Models;
using Platform.Catalog.Grpc;

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
            return CatalogIntegrationResponses.Failure(CatalogErrorCodeGrpc.InvalidProductId, "Invalid product id.");

        var product = await _unitOfWork.GetRepository<ProductModel>().FindAsync(
            x => x.Id == productId && x.Status == ProductStatus.Active,
            true,
            context.CancellationToken);

        if (product is null)
            return CatalogIntegrationResponses.Failure(CatalogErrorCodeGrpc.ProductNotFound, "Product not found.");

        return product.ToSuccessResponse();
    }

    public override async Task<DecreaseStockResponse> DecreaseStock(
        DecreaseStockRequest request,
        ServerCallContext context)
    {
        // Catalog là nơi giữ stock thật, nên checkout bên Ordering sẽ gọi
        // sang endpoint này để trừ kho thay vì tự cập nhật dữ liệu Catalog.
        foreach (var item in request.Items)
        {
            if (!Guid.TryParse(item.ProductId, out var productId))
            {
                return new DecreaseStockResponse
                {
                    IsSuccess = false,
                    ErrorCode = CatalogErrorCodeGrpc.InvalidProductId,
                    ErrorMessage = "Invalid product id."
                };
            }

            if (item.Quantity <= 0)
            {
                return new DecreaseStockResponse
                {
                    IsSuccess = false,
                    ErrorCode = CatalogErrorCodeGrpc.InvalidQuantity,
                    ErrorMessage = "Quantity must be greater than 0."
                };
            }

            var productModel = await _unitOfWork.GetRepository<ProductModel>().FindAsync(
                x => x.Id == productId && x.Status == ProductStatus.Active,
                false,
                context.CancellationToken);

            if (productModel is null)
            {
                return new DecreaseStockResponse
                {
                    IsSuccess = false,
                    ErrorCode = CatalogErrorCodeGrpc.ProductNotFound,
                    ErrorMessage = "Product not found."
                };
            }

            if (productModel is not PhysicalProductModel physicalProductModel)
                continue;

            // Luôn đổi persistence model sang domain để áp business rule ReduceStock,
            // tránh chỉnh stock trực tiếp trên model EF.
            var physicalProduct = (PhysicalProduct)physicalProductModel.ToDomain();
            var reduceStockResult = physicalProduct.ReduceStock(item.Quantity);
            if (reduceStockResult.IsFailure)
            {
                return new DecreaseStockResponse
                {
                    IsSuccess = false,
                    ErrorCode = CatalogErrorCodeGrpc.InsufficientStock,
                    ErrorMessage = reduceStockResult.Error.Message
                };
            }

            physicalProductModel.ApplyDomainState(physicalProduct);
        }

        await _unitOfWork.SaveChangesAsync(context.CancellationToken);

        return new DecreaseStockResponse
        {
            IsSuccess = true
        };
    }
}
