using Platform.Catalog.API.Application.Features.ProductCoverImages.Shared;
using Platform.Catalog.API.Domain.Entities;
using Platform.Catalog.API.Infrastructure.Persistence.Models;

namespace Platform.Catalog.API.Application.Features.ProductCoverImages.Mappers;

public static class ProductCoverImageResponseMapper
{
    public static ProductCoverImageResponse ToResponse(this ProductCoverImageModel coverImage, string? resolvedUrl)
    {
        return new ProductCoverImageResponse
        {
            BlobName = coverImage.BlobName,
            ContainerName = coverImage.ContainerName,
            FileName = coverImage.FileName,
            ContentType = coverImage.ContentType,
            Size = coverImage.Size,
            AltText = coverImage.AltText,
            Url = resolvedUrl
        };
    }

    public static ProductCoverImageResponse ToResponse(this ProductCoverImage coverImage, string? resolvedUrl = null)
    {
        return new ProductCoverImageResponse
        {
            BlobName = coverImage.BlobName,
            ContainerName = coverImage.ContainerName,
            FileName = coverImage.FileName,
            ContentType = coverImage.ContentType,
            Size = coverImage.Size,
            AltText = coverImage.AltText,
            Url = resolvedUrl ?? coverImage.Url
        };
    }
}
