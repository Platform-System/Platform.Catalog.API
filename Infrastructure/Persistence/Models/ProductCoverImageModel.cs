using System.ComponentModel.DataAnnotations.Schema;

namespace Platform.Catalog.API.Infrastructure.Persistence.Models;

[Table("ProductCoverImages")]
public sealed class ProductCoverImageModel : ProductAssetModel
{
}
