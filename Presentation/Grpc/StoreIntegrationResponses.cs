using Platform.Common.Grpc;
using Platform.Store.Grpc;

namespace Platform.Catalog.API.Presentation.Grpc;

public static class StoreIntegrationResponses
{
    public static ResolveCreateProductResponse FailureResolveCreateProduct(string errorMessage)
        => new()
        {
            Status = ResponseStatusExtensions.Failure(errorMessage)
        };

    public static ResolveCreateProductResponse SuccessResolveCreateProduct(CreateProductPolicyAction action, Guid? storeId = null)
        => new()
        {
            Status = ResponseStatusExtensions.Success(),
            Data = new ResolveCreateProductData
            {
                Action = action,
                StoreId = storeId?.ToString() ?? string.Empty
            }
        };

    public static ResolveManageProductResponse FailureResolveManageProduct(string errorMessage)
        => new()
        {
            Status = ResponseStatusExtensions.Failure(errorMessage)
        };

    public static ResolveManageProductResponse SuccessResolveManageProduct(ManageProductPolicyAction action)
        => new()
        {
            Status = ResponseStatusExtensions.Success(),
            Data = new ResolveManageProductData
            {
                Action = action
            }
        };

    public static ResolveOwnerApprovalResponse FailureResolveOwnerApproval(string errorMessage)
        => new()
        {
            Status = ResponseStatusExtensions.Failure(errorMessage)
        };

    public static ResolveOwnerApprovalResponse SuccessResolveOwnerApproval(OwnerApprovalPolicyAction action)
        => new()
        {
            Status = ResponseStatusExtensions.Success(),
            Data = new ResolveOwnerApprovalData
            {
                Action = action
            }
        };
}
