using Platform.Catalog.API.Domain.Enums;
using Platform.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace Platform.Catalog.API.Infrastructure.Persistence.Models;

[Table("ProductMedias")]
public sealed class ProductMediaModel : Entity
{
    public string FileName { get; set; } = null!;
    public string Url { get; set; } = null!;
    public string ContentType { get; set; } = null!;
    public long Size { get; set; }
    public string? AltText { get; set; }
    public MediaType Type { get; set; }
    public Guid ProductId { get; set; }
    public ProductModel Product { get; set; } = null!;
    public int SortOrder { get; set; }
}
