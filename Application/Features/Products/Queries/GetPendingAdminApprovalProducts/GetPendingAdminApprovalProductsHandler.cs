using Platform.Application.Abstractions.Storage;
using Platform.Application.Abstractions.Data;
using Platform.Application.Messaging;
using Platform.BuildingBlocks.Responses;
using Platform.Catalog.API.Application.Features.ProductCoverImages.Responses;
using Platform.Catalog.API.Application.Features.Products.Mappers;
using Platform.Catalog.API.Application.Features.Products.Responses;
using Platform.Catalog.API.Domain.Enums;
using Platform.Catalog.API.Infrastructure.Persistence.Models;

namespace Platform.Catalog.API.Application.Features.Products.Queries.GetPendingAdminApprovalProducts;

public sealed class GetPendingAdminApprovalProductsHandler : IQueryHandler<GetPendingAdminApprovalProductsQuery, PagedResult<ProductResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBlobService _blobService;

    public GetPendingAdminApprovalProductsHandler(IUnitOfWork unitOfWork, IBlobService blobService)
    {
        _unitOfWork = unitOfWork;
        _blobService = blobService;
    }

    public async Task<Result<PagedResult<ProductResponse>>> Handle(GetPendingAdminApprovalProductsQuery query, CancellationToken cancellationToken)
    {
        var products = await _unitOfWork
            .GetRepository<ProductModel>()
            .GetPagedAsync(
                query.Page,
                query.PageSize,
                x => x.Status == ProductStatus.PendingAdminReview,
                x => x.CreatedAt,
                true,
                cancellationToken,
                x => x.Category,
                x => x.CoverImage!);

        var items = products.Items
            .Select(x => x.ToResponse(x.ResolveCoverImageUrl(_blobService)))
            .ToList();

        var pagedResult = new PagedResult<ProductResponse>
        {
            Items = items,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = products.TotalCount
        };

        return Result<PagedResult<ProductResponse>>.Success(pagedResult);
    }
}
