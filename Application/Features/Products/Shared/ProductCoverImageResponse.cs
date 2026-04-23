using Platform.Catalog.API.Infrastructure.Persistence.Models;

namespace Platform.Catalog.API.Application.Features.Products.Shared;

public sealed class ProductCoverImageResponse
{
    public string BlobName { get; init; } = null!;
    public string ContainerName { get; init; } = null!;
    public string FileName { get; init; } = null!;
    public string ContentType { get; init; } = null!;
    public long Size { get; init; }

    public static ProductCoverImageResponse FromModel(ProductCoverImageModel coverImage)
    {
        return new ProductCoverImageResponse
        {
            BlobName = coverImage.BlobName,
            ContainerName = coverImage.ContainerName,
            FileName = coverImage.FileName,
            ContentType = coverImage.ContentType,
            Size = coverImage.Size
        };
    }
}
