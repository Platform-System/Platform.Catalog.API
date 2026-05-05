using Platform.Catalog.API.Application.Features.ProductMedias.Shared;
using Platform.Catalog.API.Infrastructure.Persistence.Models;

namespace Platform.Catalog.API.Application.Features.ProductMedias.Mappers;

public static class ProductMediaResponseMapper
{
    public static ProductMediaResponse ToResponse(this ProductMediaModel media, string? resolvedUrl)
    {
        return new ProductMediaResponse
        {
            Id = media.Id,
            BlobName = media.BlobName,
            ContainerName = media.ContainerName,
            FileName = media.FileName,
            Url = resolvedUrl,
            ContentType = media.ContentType,
            Size = media.Size,
            AltText = media.AltText,
            Type = media.Type,
            ProductId = media.ProductId,
            SortOrder = media.SortOrder
        };
    }
}
