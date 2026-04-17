using Platform.Catalog.API.Domain.Enums;

namespace Platform.Catalog.API.Application.Features.ProductTypes.Shared;

public sealed class ProductTypeResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public ProductTypeStatus Status { get; init; }
}
