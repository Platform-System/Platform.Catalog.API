using Platform.Catalog.API.Domain.Enums;
using Platform.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace Platform.Catalog.API.Infrastructure.Persistence.Models;

[Table("Products")]
public class ProductModel : Entity
{
    public ProductModel()
    {
    }

    public ProductModel(Guid id) : base(id)
    {
    }

    public string Title { get; set; } = null!;
    public string Author { get; set; } = null!;
    public long Price { get; set; }
    public int Stock { get; set; }
    public Guid CategoryId { get; set; }
    public Guid StoreId { get; set; }
    public ProductStatus Status { get; set; }
    public DateTime? PublishedAt { get; set; }
    public JsonDocument? AdditionalInfo { get; set; }

    public CategoryModel Category { get; set; } = null!;
    public List<ProductMediaModel> MediaFiles { get; set; } = new();
    public ProductCoverImageModel? CoverImage { get; set; }
}
