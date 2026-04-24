using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Platform.Application.Abstractions.Data;
using Platform.Catalog.API.Application.Mappers;
using Platform.Catalog.API.Domain.Entities;
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

            if (productModel is not PhysicalProductModel physicalProductModel)
                continue;

            // Luôn đổi persistence model sang domain để áp business rule ReduceStock,
            // tránh chỉnh stock trực tiếp trên model EF.
            var physicalProduct = (PhysicalProduct)physicalProductModel.ToDomain();
            var reduceStockResult = physicalProduct.ReduceStock(item.Quantity);
            if (reduceStockResult.IsFailure)
                return CatalogIntegrationResponses.FailureAdjustStock(reduceStockResult.Error.Message);

            physicalProductModel.ApplyDomainState(physicalProduct);
        }

        await _unitOfWork.SaveChangesAsync(context.CancellationToken);

        return new AdjustStockResponse
        {
            Status = ResponseStatusExtensions.Success()
        };
    }

    public override async Task<AdjustStockResponse> RestoreStock(AdjustStockRequest request, ServerCallContext context)
    {
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

            if (productModel is not PhysicalProductModel physicalProductModel)
                continue;

            // Restock vẫn đi qua domain để giữ đúng rule nghiệp vụ, không sửa thẳng model.
            var physicalProduct = (PhysicalProduct)physicalProductModel.ToDomain();
            var restockResult = physicalProduct.Restock(item.Quantity);
            if (restockResult.IsFailure)
                return CatalogIntegrationResponses.FailureAdjustStock(restockResult.Error.Message);

            physicalProductModel.ApplyDomainState(physicalProduct);
        }

        await _unitOfWork.SaveChangesAsync(context.CancellationToken);

        return new AdjustStockResponse
        {
            Status = ResponseStatusExtensions.Success()
        };
    }

    public override async Task<AuthorizeProductCoverUploadResponse> AuthorizeProductCoverUpload(
        AuthorizeProductCoverUploadRequest request,
        ServerCallContext context)
    {
        if (!Guid.TryParse(request.ProductId, out var productId))
            return CatalogIntegrationResponses.FailureAuthorizeProductCoverUpload("Invalid product id.");

        if (!Guid.TryParse(request.UserId, out var userId))
            return CatalogIntegrationResponses.FailureAuthorizeProductCoverUpload("Invalid user id.");

        var product = await _unitOfWork.GetRepository<ProductModel>().FindAsync(
            x => x.Id == productId && x.Status != ProductStatus.Deleted,
            true,
            context.CancellationToken);

        if (product is null)
            return CatalogIntegrationResponses.FailureAuthorizeProductCoverUpload("Product not found.");

        if (product.Status == ProductStatus.Active)
            return CatalogIntegrationResponses.FailureAuthorizeProductCoverUpload("Active product cover cannot be updated.");

        if (!Guid.TryParse(product.CreatedBy, out var ownerId) || ownerId != userId)
            return CatalogIntegrationResponses.FailureAuthorizeProductCoverUpload("You do not own this product.");

        var isAdmin = request.Roles.Any(role => string.Equals(role, "admin", StringComparison.OrdinalIgnoreCase));
        if (isAdmin)
        {
            return CatalogIntegrationResponses.SuccessAuthorizeProductCoverUpload(
                ProductCoverUploadVisibility.Public);
        }

        return CatalogIntegrationResponses.SuccessAuthorizeProductCoverUpload(
            ProductCoverUploadVisibility.Private);
    }

    public override async Task<SetProductCoverResponse> SetProductCover(
        SetProductCoverRequest request,
        ServerCallContext context)
    {
        if (!Guid.TryParse(request.ProductId, out var productId))
            return CatalogIntegrationResponses.FailureSetProductCover("Invalid product id.");

        if (!Guid.TryParse(request.UserId, out var userId))
            return CatalogIntegrationResponses.FailureSetProductCover("Invalid user id.");

        if (string.IsNullOrWhiteSpace(request.BlobName))
            return CatalogIntegrationResponses.FailureSetProductCover("Blob name is required.");

        if (string.IsNullOrWhiteSpace(request.ContainerName))
            return CatalogIntegrationResponses.FailureSetProductCover("Container name is required.");

        if (string.IsNullOrWhiteSpace(request.FileName))
            return CatalogIntegrationResponses.FailureSetProductCover("File name is required.");

        if (string.IsNullOrWhiteSpace(request.ContentType))
            return CatalogIntegrationResponses.FailureSetProductCover("Content type is required.");

        if (request.Size <= 0)
            return CatalogIntegrationResponses.FailureSetProductCover("File size must be greater than 0.");

        if (string.IsNullOrWhiteSpace(request.AltText))
            return CatalogIntegrationResponses.FailureSetProductCover("Alt text is required.");

        var productModel = await _unitOfWork
            .GetRepository<ProductModel>()
            .GetQueryable()
            .Include(x => x.ProductTypes)
            .Include(x => x.MediaFiles)
            .Include(x => x.CoverImage)
            .FirstOrDefaultAsync(x => x.Id == productId, context.CancellationToken);

        if (productModel is null || productModel.Status == ProductStatus.Deleted)
            return CatalogIntegrationResponses.FailureSetProductCover("Product not found.");

        if (productModel.Status == ProductStatus.Active)
            return CatalogIntegrationResponses.FailureSetProductCover("Active product cover cannot be updated.");

        if (!Guid.TryParse(productModel.CreatedBy, out var ownerId) || ownerId != userId)
            return CatalogIntegrationResponses.FailureSetProductCover("You do not own this product.");

        var product = productModel.ToDomain();
        var coverImage = request.ToCoverImage(product);
        product.SetCoverImage(coverImage);

        if (productModel.CoverImage is null)
        {
            productModel.CoverImage = coverImage.ToPersistence();
            await _unitOfWork.GetRepository<ProductCoverImageModel>().AddAsync(productModel.CoverImage, context.CancellationToken);
        }
        else
        {
            productModel.CoverImage.ApplyDomainState(coverImage);
            _unitOfWork.GetRepository<ProductCoverImageModel>().Update(productModel.CoverImage);
        }

        await _unitOfWork.SaveChangesAsync(context.CancellationToken);

        return CatalogIntegrationResponses.SuccessSetProductCover();
    }
}
