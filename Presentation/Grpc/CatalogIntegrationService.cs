using Grpc.Core;
using Platform.Application.Abstractions.Data;
using Platform.Catalog.API.Application.Features.Products.Mappers;
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

            // Luôn đổi persistence model sang domain để áp business rule ReduceStock,
            // tránh chỉnh stock trực tiếp trên model EF.
            var product = productModel.ToDomain();
            var reduceStockResult = product.ReduceStock(item.Quantity);
            if (reduceStockResult.IsFailure)
                return CatalogIntegrationResponses.FailureAdjustStock(reduceStockResult.Error.Message);

            productModel.ApplyDomainState(product);
        }

        await _unitOfWork.SaveChangesAsync(context.CancellationToken);

        return CatalogIntegrationResponses.SuccessAdjustStock();
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

            // Restock vẫn đi qua domain để giữ đúng rule nghiệp vụ, không sửa thẳng model.
            var product = productModel.ToDomain();
            var restockResult = product.Restock(item.Quantity);
            if (restockResult.IsFailure)
                return CatalogIntegrationResponses.FailureAdjustStock(restockResult.Error.Message);

            productModel.ApplyDomainState(product);
        }

        await _unitOfWork.SaveChangesAsync(context.CancellationToken);

        return CatalogIntegrationResponses.SuccessAdjustStock();
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

        if (string.IsNullOrWhiteSpace(request.Cover.BlobName))
            return CatalogIntegrationResponses.FailureSetProductCover("Blob name is required.");

        if (string.IsNullOrWhiteSpace(request.Cover.ContainerName))
            return CatalogIntegrationResponses.FailureSetProductCover("Container name is required.");

        if (string.IsNullOrWhiteSpace(request.Cover.FileName))
            return CatalogIntegrationResponses.FailureSetProductCover("File name is required.");

        if (string.IsNullOrWhiteSpace(request.Cover.ContentType))
            return CatalogIntegrationResponses.FailureSetProductCover("Content type is required.");

        if (request.Cover.Size <= 0)
            return CatalogIntegrationResponses.FailureSetProductCover("File size must be greater than 0.");

        if (string.IsNullOrWhiteSpace(request.Cover.AltText))
            return CatalogIntegrationResponses.FailureSetProductCover("Alt text is required.");

        var productModel = await _unitOfWork
            .GetRepository<ProductModel>()
            .FindAsync(
                x => x.Id == productId,
                false,
                context.CancellationToken,
                x => x.Category,
                x => x.MediaFiles,
                x => x.CoverImage!);

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

    public override async Task<SetProductMediasResponse> SetProductMedias(
        SetProductMediasRequest request,
        ServerCallContext context)
    {
        if (!Guid.TryParse(request.ProductId, out var productId))
            return CatalogIntegrationResponses.FailureSetProductMedias("Invalid product id.");

        if (!Guid.TryParse(request.UserId, out var userId))
            return CatalogIntegrationResponses.FailureSetProductMedias("Invalid user id.");

        if (request.Items == null || request.Items.Count == 0)
            return CatalogIntegrationResponses.FailureSetProductMedias("At least one media item is required.");

        var productModel = await _unitOfWork
            .GetRepository<ProductModel>()
            .FindAsync(
                x => x.Id == productId,
                false,
                context.CancellationToken,
                x => x.MediaFiles);

        if (productModel is null || productModel.Status == ProductStatus.Deleted)
            return CatalogIntegrationResponses.FailureSetProductMedias("Product not found.");

        if (productModel.Status == ProductStatus.Active)
            return CatalogIntegrationResponses.FailureSetProductMedias("Active product medias cannot be updated.");

        if (!Guid.TryParse(productModel.CreatedBy, out var ownerId) || ownerId != userId)
            return CatalogIntegrationResponses.FailureSetProductMedias("You do not own this product.");

        // 1. Chuyển persistence model sang Domain
        var product = productModel.ToDomain();

        // 2. Chuẩn bị danh sách Media mới dưới dạng Domain Entities
        var newMediaEntities = new List<ProductMedia>();
        var sortOrder = 0;
        foreach (var item in request.Items)
        {
            newMediaEntities.Add(item.ToProductMedia(productId, sortOrder++));
        }

        // 3. Thực hiện thay đổi thông qua Domain logic
        product.SetMediaFiles(newMediaEntities);

        // 4. Đồng bộ lại trạng thái từ Domain xuống Persistence layer
        // Xóa cũ
        foreach (var oldMedia in productModel.MediaFiles.ToList())
        {
            _unitOfWork.GetRepository<ProductMediaModel>().Remove(oldMedia);
        }

        // Thêm mới từ domain state
        foreach (var media in product.MediaFiles)
        {
            await _unitOfWork.GetRepository<ProductMediaModel>().AddAsync(media.ToPersistence(), context.CancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(context.CancellationToken);

        return CatalogIntegrationResponses.SuccessSetProductMedias();
    }
}

