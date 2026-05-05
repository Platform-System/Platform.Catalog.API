using Platform.Application.Messaging;
using Platform.Catalog.API.Application.Features.Stores.Shared;

namespace Platform.Catalog.API.Application.Features.Stores.Queries.GetBySlug;

public sealed class GetStoreBySlugQuery : IQuery<StoreResponse>
{
    public string Slug { get; }

    public GetStoreBySlugQuery(string slug)
    {
        Slug = slug;
    }
}
