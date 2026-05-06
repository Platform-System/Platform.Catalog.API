namespace Platform.Catalog.API.Application.Features.Stores.Services;

public enum CreateProductStorePolicyAction
{
    Allowed,
    StoreUnavailable,
    OwnerRequiredForUnverifiedStore
}

public sealed class CreateProductStorePolicyDecision
{
    public CreateProductStorePolicyAction Action { get; init; }
    public Guid StoreId { get; init; }
}

public enum ManageStoreProductPolicyAction
{
    Allowed,
    StoreUnavailable,
    CreatorInvalid,
    Forbidden
}

public sealed class ManageStoreProductPolicyDecision
{
    public ManageStoreProductPolicyAction Action { get; init; }
}

public enum OwnerStoreApprovalPolicyAction
{
    PublishActive,
    MovePendingOwnerReview,
    MovePendingAdminReview,
    UseAdminApproval,
    ForbiddenStoreMembership,
    ForbiddenCreatorOnly,
    ForbiddenOwnerOnlyUnverified,
    ForbiddenOwnerOnlyApprove,
    CreatorInvalid,
    NotReady
}

public sealed class OwnerStoreApprovalPolicyDecision
{
    public OwnerStoreApprovalPolicyAction Action { get; init; }
}
