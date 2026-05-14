using Platform.Catalog.API.Application.Features.Products.Responses;

namespace Platform.Catalog.API.Application.Features.Products.Queries.GetAll;

public sealed class GetAllProductsRequest
{
    public string? CategoryName { get; init; }
    public string? Title { get; init; }
}
