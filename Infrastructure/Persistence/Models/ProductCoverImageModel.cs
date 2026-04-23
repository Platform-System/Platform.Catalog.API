using Platform.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace Platform.Catalog.API.Infrastructure.Persistence.Models;

[Table("ProductCoverImages")]
public sealed class ProductCoverImageModel : Entity
{
    public Guid ProductId { get; set; }
    public ProductModel Product { get; set; } = null!;
    public string BlobName { get; set; } = null!;
    public string ContainerName { get; set; } = null!;
    public string FileName { get; set; } = null!;
    public string ContentType { get; set; } = null!;
    public long Size { get; set; }
    public string? Url { get; set; }
}
