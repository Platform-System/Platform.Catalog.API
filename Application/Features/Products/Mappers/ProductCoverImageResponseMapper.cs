using Platform.Catalog.API.Application.Features.Products.Shared;
using Platform.Catalog.API.Domain.Entities;
using Platform.Catalog.API.Infrastructure.Persistence.Models;

namespace Platform.Catalog.API.Application.Features.Products.Mappers;

public static class ProductCoverImageResponseMapper
{
    public static ProductCoverImageResponse ToResponse(this ProductCoverImageModel coverImage)
    {
        return new ProductCoverImageResponse
        {
            BlobName = coverImage.BlobName,
            ContainerName = coverImage.ContainerName,
            FileName = coverImage.FileName,
            ContentType = coverImage.ContentType,
            Size = coverImage.Size,
            AltText = coverImage.AltText,
            Url = coverImage.Url
        };
    }

    public static ProductCoverImageResponse ToResponse(this ProductCoverImage coverImage)
    {
        return new ProductCoverImageResponse
        {
            BlobName = coverImage.BlobName,
            ContainerName = coverImage.ContainerName,
            FileName = coverImage.FileName,
            ContentType = coverImage.ContentType,
            Size = coverImage.Size,
            AltText = coverImage.AltText,
            Url = coverImage.Url
        };
    }
}
