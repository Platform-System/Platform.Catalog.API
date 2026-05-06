using Platform.Application.Abstractions.Data;
using Platform.Catalog.API.Application.Abstractions.Stores;
using Platform.Catalog.API.Domain.Enums;
using Platform.Catalog.API.Infrastructure.Persistence.Models;

namespace Platform.Catalog.API.Application.Features.Stores.Services;

public sealed class StorePolicyService : IStorePolicyService
{
    private readonly IUnitOfWork _unitOfWork;

    public StorePolicyService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateProductStorePolicyDecision> ResolveCreateProductAsync(Guid userId, CancellationToken cancellationToken)
    {
        var storeMember = await _unitOfWork
            .GetRepository<StoreMemberModel>()
            .FindAsync(
                x => x.UserId == userId
                    && x.Status == StoreMemberStatus.Active
                    && x.Store.Status != StoreStatus.Deleted
                    && x.Store.Status != StoreStatus.Suspended,
                true,
                cancellationToken,
                x => x.Store);

        if (storeMember is null)
            return new CreateProductStorePolicyDecision { Action = CreateProductStorePolicyAction.StoreUnavailable };

        if (!storeMember.Store.IsVerified && storeMember.Role != StoreMemberRole.Owner)
            return new CreateProductStorePolicyDecision { Action = CreateProductStorePolicyAction.OwnerRequiredForUnverifiedStore };

        return new CreateProductStorePolicyDecision
        {
            Action = CreateProductStorePolicyAction.Allowed,
            StoreId = storeMember.StoreId
        };
    }

    public async Task<ManageStoreProductPolicyDecision> ResolveManageProductAsync(Guid userId, Guid storeId, string? creatorUserId, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(creatorUserId, out var creatorId))
            return new ManageStoreProductPolicyDecision { Action = ManageStoreProductPolicyAction.CreatorInvalid };

        var storeMember = await _unitOfWork
            .GetRepository<StoreMemberModel>()
            .FindAsync(
                x => x.UserId == userId
                    && x.StoreId == storeId
                    && x.Status == StoreMemberStatus.Active
                    && x.Store.Status != StoreStatus.Deleted
                    && x.Store.Status != StoreStatus.Suspended,
                true,
                cancellationToken,
                x => x.Store);

        if (storeMember is null)
            return new ManageStoreProductPolicyDecision { Action = ManageStoreProductPolicyAction.StoreUnavailable };

        return new ManageStoreProductPolicyDecision
        {
            Action = creatorId == userId || storeMember.Role == StoreMemberRole.Owner
                ? ManageStoreProductPolicyAction.Allowed
                : ManageStoreProductPolicyAction.Forbidden
        };
    }

    public async Task<OwnerStoreApprovalPolicyDecision> ResolveOwnerStoreApprovalAsync(Guid userId, Guid storeId, string? creatorUserId, ProductStatus productStatus, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(creatorUserId, out var creatorId))
            return new OwnerStoreApprovalPolicyDecision { Action = OwnerStoreApprovalPolicyAction.CreatorInvalid };

        if (productStatus == ProductStatus.PendingAdminReview)
            return new OwnerStoreApprovalPolicyDecision { Action = OwnerStoreApprovalPolicyAction.UseAdminApproval };

        var storeMember = await _unitOfWork
            .GetRepository<StoreMemberModel>()
            .FindAsync(
                x => x.StoreId == storeId
                    && x.UserId == userId
                    && x.Status == StoreMemberStatus.Active
                    && x.Store.Status != StoreStatus.Deleted
                    && x.Store.Status != StoreStatus.Suspended,
                true,
                cancellationToken,
                x => x.Store);

        if (productStatus == ProductStatus.Draft)
        {
            if (storeMember is null)
                return new OwnerStoreApprovalPolicyDecision { Action = OwnerStoreApprovalPolicyAction.ForbiddenStoreMembership };

            if (creatorId != userId)
                return new OwnerStoreApprovalPolicyDecision { Action = OwnerStoreApprovalPolicyAction.ForbiddenCreatorOnly };

            if (!storeMember.Store.IsVerified)
            {
                return new OwnerStoreApprovalPolicyDecision
                {
                    Action = storeMember.Role == StoreMemberRole.Owner
                        ? OwnerStoreApprovalPolicyAction.MovePendingAdminReview
                        : OwnerStoreApprovalPolicyAction.ForbiddenOwnerOnlyUnverified
                };
            }

            return new OwnerStoreApprovalPolicyDecision
            {
                Action = storeMember.Role == StoreMemberRole.Owner || storeMember.CanPublishProductDirectly
                    ? OwnerStoreApprovalPolicyAction.PublishActive
                    : OwnerStoreApprovalPolicyAction.MovePendingOwnerReview
            };
        }

        if (productStatus == ProductStatus.PendingOwnerReview)
        {
            return new OwnerStoreApprovalPolicyDecision
            {
                Action = storeMember is not null && storeMember.Role == StoreMemberRole.Owner
                    ? OwnerStoreApprovalPolicyAction.PublishActive
                    : OwnerStoreApprovalPolicyAction.ForbiddenOwnerOnlyApprove
            };
        }

        return new OwnerStoreApprovalPolicyDecision { Action = OwnerStoreApprovalPolicyAction.NotReady };
    }
}
