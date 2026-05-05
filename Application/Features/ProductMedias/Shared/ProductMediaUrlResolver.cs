using Platform.Application.Abstractions.Storage;
using Platform.Catalog.API.Infrastructure.Persistence.Models;

namespace Platform.Catalog.API.Application.Features.ProductMedias.Shared;

public static class ProductMediaUrlResolver
{
    public static string? ResolveUrl(this ProductMediaModel media, IBlobService blobService)
    {
        if (!string.IsNullOrWhiteSpace(media.Url))
        {
            return media.Url;
        }

        if (string.IsNullOrWhiteSpace(media.ContainerName) || string.IsNullOrWhiteSpace(media.BlobName))
        {
            return null;
        }

        return blobService.GenerateReadSasUrl(media.ContainerName, media.BlobName);
    }
}
