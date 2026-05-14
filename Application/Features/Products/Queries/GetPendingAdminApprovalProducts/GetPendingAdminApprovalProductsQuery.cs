using Platform.Application.Messaging;
using Platform.BuildingBlocks.Requests;
using Platform.BuildingBlocks.Responses;
using Platform.Catalog.API.Application.Features.Products.Responses;

namespace Platform.Catalog.API.Application.Features.Products.Queries.GetPendingAdminApprovalProducts;

public sealed class GetPendingAdminApprovalProductsQuery : PagingRequest, IQuery<PagedResult<ProductResponse>>
{
}
