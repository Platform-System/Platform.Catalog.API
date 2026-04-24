using Platform.Domain.Common;

namespace Platform.Catalog.API.Infrastructure.Persistence.Models;

public abstract class ProductAssetModel : Entity
{
    public Guid ProductId { get; set; }
    public ProductModel Product { get; set; } = null!;
    public string BlobName { get; set; } = null!;
    public string ContainerName { get; set; } = null!;
    public string FileName { get; set; } = null!;
    public string ContentType { get; set; } = null!;
    public long Size { get; set; }
    public string? Url { get; set; }
    public string? AltText { get; set; }
}
