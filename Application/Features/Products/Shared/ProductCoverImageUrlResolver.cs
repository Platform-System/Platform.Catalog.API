using Platform.Application.Abstractions.Storage;
using Platform.Catalog.API.Infrastructure.Persistence.Models;

namespace Platform.Catalog.API.Application.Features.Products.Shared;

public static class ProductCoverImageUrlResolver
{
    public static string? ResolveCoverImageUrl(this ProductModel product, IBlobService blobService)
    {
        if (product.CoverImage is null)
        {
            return null;
        }

        return string.IsNullOrWhiteSpace(product.CoverImage.Url)
            ? blobService.GenerateReadSasUrl(product.CoverImage.ContainerName, product.CoverImage.BlobName)
            : product.CoverImage.Url;
    }
}
