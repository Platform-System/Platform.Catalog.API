using Platform.Application.Abstractions.Data;
using Platform.Application.Abstractions.Storage;
using Platform.Application.Messaging;
using Platform.BuildingBlocks.Abstractions;
using Platform.BuildingBlocks.Responses;
using Platform.Catalog.API.Application.Features.Products.Mappers;
using Platform.Catalog.API.Application.Features.Products.Shared;
using Platform.Catalog.API.Domain.Enums;
using Platform.Catalog.API.Infrastructure.Persistence.Models;

namespace Platform.Catalog.API.Application.Features.Stores.Queries.GetPendingOwnerReview;

public sealed class GetPendingOwnerReviewHandler : IQueryHandler<GetPendingOwnerReviewQuery, PagedResult<ProductResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IBlobService _blobService;

    public GetPendingOwnerReviewHandler(IUnitOfWork unitOfWork, ICurrentUserProvider currentUserProvider, IBlobService blobService)
    {
        _unitOfWork = unitOfWork;
        _currentUserProvider = currentUserProvider;
        _blobService = blobService;
    }

    public async Task<Result<PagedResult<ProductResponse>>> Handle(GetPendingOwnerReviewQuery query, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(_currentUserProvider.CurrentUserId, out var currentUserId))
            return Result<PagedResult<ProductResponse>>.Failure(StatusCodes.Status401Unauthorized, "Current user is invalid.");

        var ownerMember = await _unitOfWork
            .GetRepository<StoreMemberModel>()
            .FindAsync(
                x => x.UserId == currentUserId
                    && x.Role == StoreMemberRole.Owner
                    && x.Status == StoreMemberStatus.Active
                    && x.Store.Status != StoreStatus.Deleted,
                true,
                cancellationToken,
                x => x.Store);

        if (ownerMember is null)
            return Result<PagedResult<ProductResponse>>.Failure(StatusCodes.Status404NotFound, "Store not found.");

        var products = await _unitOfWork
            .GetRepository<ProductModel>()
            .GetPagedAsync(
                query.Page,
                query.PageSize,
                x => x.StoreId == ownerMember.StoreId
                    && x.Status == ProductStatus.PendingOwnerReview,
                x => x.CreatedAt,
                true,
                cancellationToken,
                x => x.Category,
                x => x.CoverImage!);

        var pagedResult = new PagedResult<ProductResponse>
        {
            Items = products.Items.Select(x => x.ToResponse(x.ResolveCoverImageUrl(_blobService))).ToList(),
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = products.TotalCount
        };

        return Result<PagedResult<ProductResponse>>.Success(pagedResult);
    }
}
