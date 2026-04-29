using Platform.Domain.Common;
using Platform.Catalog.API.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Platform.Catalog.API.Infrastructure.Persistence.Models;

[Table("Categories")]
public sealed class CategoryModel : Entity
{
    public CategoryModel()
    {
    }

    public CategoryModel(Guid id) : base(id)
    {
    }

    public string Name { get; set; } = null!;
    public CategoryStatus Status { get; set; }

    public List<ProductModel> Products { get; set; } = new();
}
