namespace Platform.Catalog.API.Application.Abstractions.Stores;

public enum CreateProductStorePolicyAction
{
    Allowed,
    StoreUnavailable,
    StoreNotActive
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
    StoreNotActive,
    ForbiddenStoreMembership,
    ForbiddenCreatorOnly,
    ForbiddenOwnerOnlyApprove,
    CreatorInvalid,
    NotReady
}

public sealed class OwnerStoreApprovalPolicyDecision
{
    public OwnerStoreApprovalPolicyAction Action { get; init; }
}
