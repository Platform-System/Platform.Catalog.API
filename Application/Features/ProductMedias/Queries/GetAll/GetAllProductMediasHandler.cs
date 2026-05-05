using System.Linq.Expressions;
using Platform.Application.Abstractions.Data;
using Platform.Application.Abstractions.Storage;
using Platform.Application.Messaging;
using Platform.BuildingBlocks.Responses;
using Platform.Catalog.API.Application.Features.ProductMedias.Mappers;
using Platform.Catalog.API.Application.Features.ProductMedias.Shared;
using Platform.Catalog.API.Infrastructure.Persistence.Models;

namespace Platform.Catalog.API.Application.Features.ProductMedias.Queries.GetAll;

public sealed class GetAllProductMediasHandler : IQueryHandler<GetAllProductMediasQuery, PagedResult<ProductMediaResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBlobService _blobService;

    public GetAllProductMediasHandler(IUnitOfWork unitOfWork, IBlobService blobService)
    {
        _unitOfWork = unitOfWork;
        _blobService = blobService;
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

        var items = productMedias.Items.Select(media => media.ToResponse(media.ResolveUrl(_blobService))).ToList();

        var pagedResult = new PagedResult<ProductMediaResponse>
        {
            Items = items,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = productMedias.TotalCount
        };

        return Result<PagedResult<ProductMediaResponse>>.Success(pagedResult);
    }
}
