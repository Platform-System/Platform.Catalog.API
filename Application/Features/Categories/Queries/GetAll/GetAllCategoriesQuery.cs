using Platform.Application.Messaging;
using Platform.BuildingBlocks.Requests;
using Platform.BuildingBlocks.Responses;
using Platform.Catalog.API.Application.Features.Categories.Shared;

namespace Platform.Catalog.API.Application.Features.Categories.Queries.GetAll;

public sealed class GetAllCategoriesQuery : PagingRequest, IQuery<PagedResult<CategoryResponse>>
{
}
