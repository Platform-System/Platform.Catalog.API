namespace Platform.Catalog.API.Application.Features.Products.Commands.UploadImage;

public sealed class UploadImageRequest
{
    public required Stream Stream { get; init; }
    public required string FileName { get; init; }
    public required string ContentType { get; init; }
}
