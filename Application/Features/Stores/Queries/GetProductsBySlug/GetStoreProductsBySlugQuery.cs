using Platform.Application.Messaging;
using Platform.BuildingBlocks.Responses;
using Platform.Catalog.API.Application.Features.Products.Shared;

namespace Platform.Catalog.API.Application.Features.Stores.Queries.GetProductsBySlug;

public sealed class GetStoreProductsBySlugQuery : IQuery<PagedResult<ProductResponse>>
{
    public string Slug { get; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;

    public GetStoreProductsBySlugQuery(string slug)
    {
        Slug = slug;
    }
}
