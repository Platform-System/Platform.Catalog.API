using Platform.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace Platform.Catalog.API.Infrastructure.Persistence.Models;

[Table("ProductTypes")]
public sealed class ProductTypeModel : Entity
{
    public string Name { get; set; } = null!;

    public List<ProductModel> Products { get; set; } = new();
}
