namespace Platform.Catalog.API.Application.Features.Products.Commands.SetMedias;

public sealed class SetProductMediasRequest
{
    public IReadOnlyCollection<SetProductMediaItemRequest> Items { get; init; } = [];
}

public sealed class SetProductMediaItemRequest
{
    public string BlobName { get; init; } = null!;
    public string ContainerName { get; init; } = null!;
    public string FileName { get; init; } = null!;
    public string ContentType { get; init; } = null!;
    public long Size { get; init; }
    public string AltText { get; init; } = null!;
}
