namespace Platform.Catalog.API.Infrastructure.Persistence.Models;

public sealed class PhysicalProductModel : ProductModel
{
    public PhysicalProductModel()
    {
    }

    public PhysicalProductModel(Guid id, int stock) : base(id)
    {
        Stock = stock;
    }

    public int Stock { get; set; }
}
