using Platform.Catalog.API.Infrastructure.Persistence.Models;

namespace Platform.Catalog.API.Application.Features.ProductMedias.Shared;

public static class ProductMediaMapper
{
    public static ProductMediaResponse ToResponse(this ProductMediaModel media)
    {
        return new ProductMediaResponse
        {
            Id = media.Id,
            FileName = media.FileName,
            Url = media.Url,
            ContentType = media.ContentType,
            Size = media.Size,
            AltText = media.AltText,
            Type = media.Type,
            ProductId = media.ProductId,
            SortOrder = media.SortOrder
        };
    }
}
