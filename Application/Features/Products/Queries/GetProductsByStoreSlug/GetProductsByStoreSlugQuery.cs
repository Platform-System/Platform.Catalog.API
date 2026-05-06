using Platform.Application.Messaging;
using Platform.BuildingBlocks.Requests;
using Platform.BuildingBlocks.Responses;
using Platform.Catalog.API.Application.Features.Products.Shared;

namespace Platform.Catalog.API.Application.Features.Products.Queries.GetProductsByStoreSlug;

public sealed class GetProductsByStoreSlugQuery : PagingRequest, IQuery<PagedResult<ProductResponse>>
{
    public string Slug { get; }

    public GetProductsByStoreSlugQuery(string slug)
    {
        Slug = slug;
    }
}
