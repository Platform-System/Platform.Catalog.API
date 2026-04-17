using Platform.Application.Messaging;
using Platform.BuildingBlocks.Requests;
using Platform.BuildingBlocks.Responses;
using Platform.Catalog.API.Application.Features.ProductTypes.Shared;

namespace Platform.Catalog.API.Application.Features.ProductTypes.Queries.GetAll;

public sealed class GetAllProductTypesQuery : PagingRequest, IQuery<PagedResult<ProductTypeResponse>>
{
}
