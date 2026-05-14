using Platform.Application.Abstractions.Data;
using Platform.Application.Abstractions.Storage;
using Platform.Application.Messaging;
using Platform.BuildingBlocks.Abstractions;
using Platform.BuildingBlocks.Responses;
using Platform.Catalog.API.Application.Abstractions.Stores;
using Platform.Catalog.API.Application.Features.ProductCoverImages.Responses;
using Platform.Catalog.API.Application.Features.Products.Mappers;
using Platform.Catalog.API.Application.Features.Products.Responses;
using Platform.Catalog.API.Domain.Enums;
using Platform.Catalog.API.Infrastructure.Persistence.Models;

namespace Platform.Catalog.API.Application.Features.Products.Queries.GetPendingOwnerApprovalProducts;

public sealed class GetPendingOwnerApprovalProductsHandler : IQueryHandler<GetPendingOwnerApprovalProductsQuery, PagedResult<ProductResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IBlobService _blobService;
    private readonly IStoreReadService _storeReadService;

    public GetPendingOwnerApprovalProductsHandler(IUnitOfWork unitOfWork, ICurrentUserProvider currentUserProvider, IBlobService blobService, IStoreReadService storeReadService)
    {
        _unitOfWork = unitOfWork;
        _currentUserProvider = currentUserProvider;
        _blobService = blobService;
        _storeReadService = storeReadService;
    }

    public async Task<Result<PagedResult<ProductResponse>>> Handle(GetPendingOwnerApprovalProductsQuery query, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(_currentUserProvider.CurrentUserId, out var currentUserId))
            return Result<PagedResult<ProductResponse>>.Failure(StatusCodes.Status401Unauthorized, "Current user is invalid.");

        var storeId = await _storeReadService.GetCurrentOwnerStoreIdAsync(currentUserId, cancellationToken);
        if (storeId is null)
            return Result<PagedResult<ProductResponse>>.Failure(StatusCodes.Status404NotFound, "Store not found.");

        var products = await _unitOfWork
            .GetRepository<ProductModel>()
            .GetPagedAsync(
                query.Page,
                query.PageSize,
                x => x.StoreId == storeId.Value
                    && x.Status == ProductStatus.PendingOwnerReview,
                x => x.CreatedAt,
                true,
                cancellationToken,
                x => x.Category,
                x => x.CoverImage!);

        return Result<PagedResult<ProductResponse>>.Success(new PagedResult<ProductResponse>
        {
            Items = products.Items.Select(x => x.ToResponse(x.ResolveCoverImageUrl(_blobService))).ToList(),
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = products.TotalCount
        });
    }
}
