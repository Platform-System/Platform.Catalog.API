using Platform.Catalog.API.Application.Features.Products.Shared;

namespace Platform.Catalog.API.Application.Features.Products.Commands.Create;

public sealed class CreateProductRequest
{
    public string Title { get; init; } = null!;
    public string Author { get; init; } = null!;
    public long Price { get; init; }
    public Guid CategoryId { get; init; }
    public int Stock { get; init; }
}
