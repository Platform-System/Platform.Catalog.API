using Platform.Application.Abstractions.Data;
using Platform.Application.Messaging;
using Platform.BuildingBlocks.Responses;
using Platform.Catalog.API.Application.Features.ProductTypes.Shared;
using Platform.Catalog.API.Domain.Enums;
using Platform.Catalog.API.Infrastructure.Persistence.Models;

namespace Platform.Catalog.API.Application.Features.ProductTypes.Queries.GetAll;

public sealed class GetAllProductTypesHandler : IQueryHandler<GetAllProductTypesQuery, PagedResult<ProductTypeResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllProductTypesHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PagedResult<ProductTypeResponse>>> Handle(GetAllProductTypesQuery query, CancellationToken cancellationToken)
    {
        var productTypes = await _unitOfWork
            .GetRepository<ProductTypeModel>()
            .GetPagedAsync(
                query.Page,
                query.PageSize,
                x => x.Status == ProductTypeStatus.Active,
                orderBy: x => x.Name,
                isDescending: false,
                cancellationToken: cancellationToken);

        var pagedResult = new PagedResult<ProductTypeResponse>
        {
            Items = productTypes.Items.Select(x => x.ToResponse()).ToList(),
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = productTypes.TotalCount
        };

        return Result<PagedResult<ProductTypeResponse>>.Success(pagedResult);
    }
}
