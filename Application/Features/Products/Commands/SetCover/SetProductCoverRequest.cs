namespace Platform.Catalog.API.Application.Features.Products.Commands.SetCover;

public sealed class SetProductCoverRequest
{
    public string BlobName { get; init; } = null!;
    public string ContainerName { get; init; } = null!;
    public string FileName { get; init; } = null!;
    public string ContentType { get; init; } = null!;
    public long Size { get; init; }
}
