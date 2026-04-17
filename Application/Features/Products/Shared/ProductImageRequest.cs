namespace Platform.Catalog.API.Application.Features.Products.Shared;

public sealed class ProductImageRequest
{
    public required Stream Stream { get; init; }
    public required string FileName { get; init; }
    public required string ContentType { get; init; }
}
