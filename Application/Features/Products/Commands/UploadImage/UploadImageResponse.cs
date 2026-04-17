namespace Platform.Catalog.API.Application.Features.Products.Commands.UploadImage;

public sealed class UploadImageResponse
{
    public string BlobName { get; init; } = null!;
    public string ContainerName { get; init; } = null!;
}
