using Platform.Application.Messaging;
using Platform.BuildingBlocks.Requests;
using Platform.BuildingBlocks.Responses;
using Platform.Catalog.API.Application.Features.Products.Shared;

namespace Platform.Catalog.API.Application.Features.Products.Queries.PendingProductOfUser;

public sealed class GetPendingProductOfUserQuery : PagingRequest, IQuery<PagedResult<ProductResponse>>
{
}
