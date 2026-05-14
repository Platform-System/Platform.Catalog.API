namespace Platform.Catalog.API.Application.Features.ProductCoverImages.Responses;

public sealed class ProductCoverImageResponse
{
    public string BlobName { get; init; } = null!;
    public string ContainerName { get; init; } = null!;
    public string FileName { get; init; } = null!;
    public string ContentType { get; init; } = null!;
    public long Size { get; init; }
    public string? AltText { get; init; }
    public string? Url { get; init; }
}
