using Platform.Application.Messaging;
using Platform.BuildingBlocks.Requests;
using Platform.BuildingBlocks.Responses;
using Platform.Catalog.API.Application.Features.Products.Shared;

namespace Platform.Catalog.API.Application.Features.Products.Queries.GetAll;

public sealed class GetAllProductsQuery : PagingRequest, IQuery<PagedResult<ProductResponse>>
{
    public GetAllProductsRequest Request { get; }
    public GetAllProductsQuery(GetAllProductsRequest request)
    {
        Request = request;
    }

}
