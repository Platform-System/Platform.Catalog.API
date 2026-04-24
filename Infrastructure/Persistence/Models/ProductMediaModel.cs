using Platform.Catalog.API.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Platform.Catalog.API.Infrastructure.Persistence.Models;

[Table("ProductMedias")]
public sealed class ProductMediaModel : ProductAssetModel
{
    public MediaType Type { get; set; }
    public int SortOrder { get; set; }
}
