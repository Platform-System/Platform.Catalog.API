using Platform.Application.Messaging;
using Platform.BuildingBlocks.Requests;
using Platform.BuildingBlocks.Responses;
using Platform.Catalog.API.Application.Features.Products.Shared;

namespace Platform.Catalog.API.Application.Features.Stores.Queries.GetProductsBySlug;

public sealed class GetStoreProductsBySlugQuery : PagingRequest, IQuery<PagedResult<ProductResponse>>
{
    public string Slug { get; }

    public GetStoreProductsBySlugQuery(string slug)
    {
        Slug = slug;
    }
}
