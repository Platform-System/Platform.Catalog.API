using Platform.Catalog.API.Application.Features.Products.Shared;

using Platform.SharedKernel.Enums;

namespace Platform.Catalog.API.Application.Features.Products.Queries.GetAll;

public sealed class GetAllProductsRequest
{
    public string? ProductTypeName { get; init; }
    public string? Title { get; init; }
    public ProductKind? Kind { get; init; }
}
