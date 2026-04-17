using Platform.Catalog.API.Application.Features.Products.Shared;

namespace Platform.Catalog.API.Application.Features.Products.Commands.Update;

public sealed class UpdateProductRequest
{
    public ProductKind Kind { get; init; }
    public string Title { get; init; } = null!;
    public string Author { get; init; } = null!;
    public long Price { get; init; }
    public IReadOnlyCollection<Guid> ProductTypeIds { get; init; } = [];
    public int? Stock { get; init; }
}
