using Platform.Catalog.API.Application.Features.Products.Shared;

using Platform.SharedKernel.Enums;

namespace Platform.Catalog.API.Application.Features.Products.Commands.Create;

public sealed class CreateProductRequest
{
    public ProductKind Kind { get; init; }
    public string Title { get; init; } = null!;
    public string Author { get; init; } = null!;
    public long Price { get; init; }
    public IReadOnlyCollection<Guid> ProductTypeIds { get; init; } = [];
    public int? Stock { get; init; }
}
