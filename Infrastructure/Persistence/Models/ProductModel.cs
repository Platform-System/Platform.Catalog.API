using Platform.Catalog.API.Domain.Enums;
using Platform.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace Platform.Catalog.API.Infrastructure.Persistence.Models;

[Table("Products")]
public abstract class ProductModel : Entity
{
    protected ProductModel()
    {
    }

    protected ProductModel(Guid id) : base(id)
    {
    }

    public string Title { get; set; } = null!;
    public string Author { get; set; } = null!;
    public long Price { get; set; }
    public ProductStatus Status { get; set; }
    public DateTime? PublishedAt { get; set; }
    public JsonDocument? AdditionalInfo { get; set; }

    public List<ProductTypeModel> ProductTypes { get; set; } = new();
    public List<ProductMediaModel> MediaFiles { get; set; } = new();
    public ProductCoverImageModel? CoverImage { get; set; }
}
