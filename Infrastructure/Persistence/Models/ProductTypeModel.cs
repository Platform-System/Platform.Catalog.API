using Platform.Domain.Common;
using Platform.Catalog.API.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Platform.Catalog.API.Infrastructure.Persistence.Models;

[Table("ProductTypes")]
public sealed class ProductTypeModel : Entity
{
    public ProductTypeModel()
    {
    }

    public ProductTypeModel(Guid id) : base(id)
    {
    }

    public string Name { get; set; } = null!;
    public ProductTypeStatus Status { get; set; }

    public List<ProductModel> Products { get; set; } = new();
}
