using Platform.Catalog.API.Domain.Enums;

namespace Platform.Catalog.API.Application.Features.ProductMedias.Shared;

public sealed class ProductMediaResponse
{
    public Guid Id { get; init; }
    public string BlobName { get; init; } = null!;
    public string ContainerName { get; init; } = null!;
    public string FileName { get; init; } = null!;
    public string? Url { get; init; }
    public string ContentType { get; init; } = null!;
    public long Size { get; init; }
    public string? AltText { get; init; }
    public MediaType Type { get; init; }
    public Guid ProductId { get; init; }
    public int SortOrder { get; init; }
}
