using Platform.Application.Messaging;
using Platform.BuildingBlocks.Requests;
using Platform.BuildingBlocks.Responses;
using Platform.Catalog.API.Application.Features.ProductMedias.Shared;

namespace Platform.Catalog.API.Application.Features.ProductMedias.Queries.GetAll;

public sealed class GetAllProductMediasQuery : PagingRequest, IQuery<PagedResult<ProductMediaResponse>>
{
    public GetAllProductMediasRequest Request { get; }
    public GetAllProductMediasQuery(GetAllProductMediasRequest request)
    {
        Request = request;
    }

}
