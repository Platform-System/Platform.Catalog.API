using Platform.Application.Abstractions.Data;
using Platform.Application.Messaging;
using Platform.BuildingBlocks.Responses;
using Platform.Catalog.API.Application.Features.Categories.Mappers;
using Platform.Catalog.API.Application.Features.Categories.Shared;
using Platform.Catalog.API.Domain.Enums;
using Platform.Catalog.API.Infrastructure.Persistence.Models;

namespace Platform.Catalog.API.Application.Features.Categories.Queries.GetAll;

public sealed class GetAllCategoriesHandler : IQueryHandler<GetAllCategoriesQuery, PagedResult<CategoryResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllCategoriesHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PagedResult<CategoryResponse>>> Handle(GetAllCategoriesQuery query, CancellationToken cancellationToken)
    {
        var categories = await _unitOfWork
            .GetRepository<CategoryModel>()
            .GetPagedAsync(
                query.Page,
                query.PageSize,
                x => x.Status == CategoryStatus.Active,
                orderBy: x => x.Name,
                isDescending: false,
                cancellationToken: cancellationToken);

        var pagedResult = new PagedResult<CategoryResponse>
        {
            Items = categories.Items.Select(x => x.ToResponse()).ToList(),
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = categories.TotalCount
        };

        return Result<PagedResult<CategoryResponse>>.Success(pagedResult);
    }
}
