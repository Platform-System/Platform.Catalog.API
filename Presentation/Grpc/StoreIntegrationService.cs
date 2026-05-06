using Grpc.Core;
using Platform.Catalog.API.Application.Abstractions.Stores;
using Platform.Catalog.API.Domain.Enums;
using Platform.Store.Grpc;

namespace Platform.Catalog.API.Presentation.Grpc;

public sealed class StoreIntegrationService : StoreIntegration.StoreIntegrationBase
{
    private readonly IStorePolicyService _storePolicyService;

    public StoreIntegrationService(IStorePolicyService storePolicyService)
    {
        _storePolicyService = storePolicyService;
    }

    public override async Task<ResolveCreateProductResponse> ResolveCreateProduct(ResolveCreateProductRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.UserId, out var userId))
            return StoreIntegrationResponses.FailureResolveCreateProduct("Invalid user id.");

        var decision = await _storePolicyService.ResolveCreateProductAsync(userId, context.CancellationToken);
        return StoreIntegrationResponses.SuccessResolveCreateProduct(
            decision.Action switch
            {
                CreateProductStorePolicyAction.Allowed => (CreateProductPolicyAction)1,
                CreateProductStorePolicyAction.StoreUnavailable => (CreateProductPolicyAction)2,
                CreateProductStorePolicyAction.OwnerRequiredForUnverifiedStore => (CreateProductPolicyAction)3,
                _ => (CreateProductPolicyAction)0
            },
            decision.Action == CreateProductStorePolicyAction.Allowed ? decision.StoreId : null);
    }

    public override async Task<ResolveManageProductResponse> ResolveManageProduct(ResolveManageProductRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.UserId, out var userId))
            return StoreIntegrationResponses.FailureResolveManageProduct("Invalid user id.");

        if (!Guid.TryParse(request.StoreId, out var storeId))
            return StoreIntegrationResponses.FailureResolveManageProduct("Invalid store id.");

        var decision = await _storePolicyService.ResolveManageProductAsync(
            userId,
            storeId,
            request.CreatorUserId,
            context.CancellationToken);

        return StoreIntegrationResponses.SuccessResolveManageProduct(
            decision.Action switch
            {
                ManageStoreProductPolicyAction.Allowed => (ManageProductPolicyAction)1,
                ManageStoreProductPolicyAction.StoreUnavailable => (ManageProductPolicyAction)2,
                ManageStoreProductPolicyAction.CreatorInvalid => (ManageProductPolicyAction)3,
                ManageStoreProductPolicyAction.Forbidden => (ManageProductPolicyAction)4,
                _ => (ManageProductPolicyAction)0
            });
    }

    public override async Task<ResolveOwnerApprovalResponse> ResolveOwnerApproval(ResolveOwnerApprovalRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.UserId, out var userId))
            return StoreIntegrationResponses.FailureResolveOwnerApproval("Invalid user id.");

        if (!Guid.TryParse(request.StoreId, out var storeId))
            return StoreIntegrationResponses.FailureResolveOwnerApproval("Invalid store id.");

        var decision = await _storePolicyService.ResolveOwnerStoreApprovalAsync(
            userId,
            storeId,
            request.CreatorUserId,
            request.ProductStatus.ToDomain(),
            context.CancellationToken);

        return StoreIntegrationResponses.SuccessResolveOwnerApproval(
            decision.Action switch
            {
                OwnerStoreApprovalPolicyAction.PublishActive => (OwnerApprovalPolicyAction)1,
                OwnerStoreApprovalPolicyAction.MovePendingOwnerReview => (OwnerApprovalPolicyAction)2,
                OwnerStoreApprovalPolicyAction.MovePendingAdminReview => (OwnerApprovalPolicyAction)3,
                OwnerStoreApprovalPolicyAction.UseAdminApproval => (OwnerApprovalPolicyAction)4,
                OwnerStoreApprovalPolicyAction.ForbiddenStoreMembership => (OwnerApprovalPolicyAction)5,
                OwnerStoreApprovalPolicyAction.ForbiddenCreatorOnly => (OwnerApprovalPolicyAction)6,
                OwnerStoreApprovalPolicyAction.ForbiddenOwnerOnlyUnverified => (OwnerApprovalPolicyAction)7,
                OwnerStoreApprovalPolicyAction.ForbiddenOwnerOnlyApprove => (OwnerApprovalPolicyAction)8,
                OwnerStoreApprovalPolicyAction.CreatorInvalid => (OwnerApprovalPolicyAction)9,
                OwnerStoreApprovalPolicyAction.NotReady => (OwnerApprovalPolicyAction)10,
                _ => (OwnerApprovalPolicyAction)0
            });
    }
}

file static class StoreIntegrationMappings
{
    public static ProductStatus ToDomain(this ProductApprovalStatus status)
    {
        return status switch
        {
            (ProductApprovalStatus)1 => ProductStatus.Draft,
            (ProductApprovalStatus)2 => ProductStatus.PendingOwnerReview,
            (ProductApprovalStatus)3 => ProductStatus.PendingAdminReview,
            (ProductApprovalStatus)4 => ProductStatus.Active,
            (ProductApprovalStatus)5 => ProductStatus.Inactive,
            (ProductApprovalStatus)6 => ProductStatus.Deleted,
            _ => ProductStatus.Draft
        };
    }
}
