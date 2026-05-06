using Platform.Catalog.API.Domain.Enums;

namespace Platform.Catalog.API.Application.Abstractions.Stores;

public interface IStorePolicyService
{
    Task<CreateProductStorePolicyDecision> ResolveCreateProductAsync(Guid userId, CancellationToken cancellationToken);
    Task<ManageStoreProductPolicyDecision> ResolveManageProductAsync(Guid userId, Guid storeId, string? creatorUserId, CancellationToken cancellationToken);
    Task<OwnerStoreApprovalPolicyDecision> ResolveOwnerStoreApprovalAsync(Guid userId, Guid storeId, string? creatorUserId, ProductStatus productStatus, CancellationToken cancellationToken);
}
