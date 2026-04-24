using Platform.Catalog.API.Domain.Entities;
using Platform.Catalog.API.Domain.Enums;
using Platform.Catalog.API.Infrastructure.Persistence.Models;
using Platform.Catalog.Grpc;
using Platform.Common.Grpc;

namespace Platform.Catalog.API.Presentation.Grpc;

public static class CatalogIntegrationMapper
{
    public static GetProductCartSnapshotResponse ToSuccessResponse(this ProductModel product)
    {
        // Chỉ trả các field Ordering cần để kiểm tra rule của cart.
        var kind = product is PhysicalProductModel
            ? ProductKindGrpc.Physical
            : ProductKindGrpc.Digital;

        var stock = product is PhysicalProductModel physicalProduct
            ? physicalProduct.Stock
            : 0;

        return new GetProductCartSnapshotResponse
        {
            Status = ResponseStatusExtensions.Success(),
            Data = new ProductCartSnapshotData
            {
                Id = product.Id.ToString(),
                Title = product.Title,
                Price = product.Price,
                Kind = kind,
                IsActive = product.Status == ProductStatus.Active,
                HasStock = product is PhysicalProductModel,
                Stock = stock
            }
        };
    }

    public static ProductCoverImage ToCoverImage(this SetProductCoverRequest request, Product product)
    {
        if (product.CoverImage is null)
        {
            return new ProductCoverImage(
                product.Id,
                request.Cover.BlobName,
                request.Cover.ContainerName,
                request.Cover.FileName,
                request.Cover.ContentType,
                request.Cover.Size,
                request.Cover.AltText);
        }

        product.CoverImage.UpdateMetadata(
            request.Cover.BlobName,
            request.Cover.ContainerName,
            request.Cover.FileName,
            request.Cover.ContentType,
            request.Cover.Size,
            request.Cover.AltText);

        return product.CoverImage;
    }

    public static ProductMedia ToProductMedia(this UploadedFileInfo file, Guid productId, int sortOrder = 0)
    {
        return new ProductMedia(
            productId,
            file.BlobName,
            file.ContainerName,
            file.FileName,
            file.ContentType,
            file.Size,
            MediaType.Image, // Default to Image for now
            sortOrder,
            file.AltText);
    }
}
