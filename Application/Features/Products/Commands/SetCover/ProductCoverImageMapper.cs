using Platform.Catalog.API.Domain.Entities;

namespace Platform.Catalog.API.Application.Features.Products.Commands.SetCover;

public static class ProductCoverImageMapper
{
    public static ProductCoverImage ToCoverImage(this SetProductCoverRequest request, Product product)
    {
        if (product.CoverImage is null)
        {
            return new ProductCoverImage(
                product.Id,
                request.BlobName,
                request.ContainerName,
                request.FileName,
                request.ContentType,
                request.Size,
                request.AltText);
        }

        product.CoverImage.UpdateMetadata(
            request.BlobName,
            request.ContainerName,
            request.FileName,
            request.ContentType,
            request.Size,
            request.AltText);

        return product.CoverImage;
    }
}
