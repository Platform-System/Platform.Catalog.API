using System.Linq.Expressions;
using Platform.Application.Abstractions.Data;
using Platform.Application.Messaging;
using Platform.BuildingBlocks.Responses;
using Platform.Catalog.API.Application.Features.ProductMedias.Shared;
using Platform.Catalog.API.Infrastructure.Persistence.Models;

namespace Platform.Catalog.API.Application.Features.ProductMedias.Queries.GetAll;

public sealed class GetAllProductMediasHandler : IQueryHandler<GetAllProductMediasQuery, PagedResult<ProductMediaResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllProductMediasHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PagedResult<ProductMediaResponse>>> Handle(GetAllProductMediasQuery query, CancellationToken cancellationToken)
    {
        Expression<Func<ProductMediaModel, bool>> filter = x => true;

        if (query.Request.ProductId.HasValue)
        {
            var productId = query.Request.ProductId.Value;
            filter = x => x.ProductId == productId;
        }

        var productMedias = await _unitOfWork
            .GetRepository<ProductMediaModel>()
            .GetPagedAsync(
                query.Page,
                query.PageSize,
                filter,
                x => x.SortOrder,
                false,
                cancellationToken);

        var pagedResult = new PagedResult<ProductMediaResponse>
        {
            Items = productMedias.Items.Select(x => x.ToResponse()).ToList(),
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = productMedias.TotalCount
        };

        return Result<PagedResult<ProductMediaResponse>>.Success(pagedResult);
    }
}
