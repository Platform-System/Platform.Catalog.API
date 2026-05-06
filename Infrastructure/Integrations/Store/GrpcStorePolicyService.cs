using Grpc.Core;
using Microsoft.Extensions.Options;
using Platform.Catalog.API.Application.Abstractions.Stores;
using Platform.Catalog.API.Domain.Enums;
using Platform.Common.Grpc;
using Platform.StoreService.Grpc;

namespace Platform.Catalog.API.Infrastructure.Integrations.Store;

public sealed class GrpcStorePolicyService : IStorePolicyService
{
    private readonly StoreIntegration.StoreIntegrationClient _client;
    private readonly StoreClientOptions _options;

    public GrpcStorePolicyService(StoreIntegration.StoreIntegrationClient client, IOptions<StoreClientOptions> options)
    {
        _client = client;
        _options = options.Value;
    }

    public async Task<CreateProductStorePolicyDecision> ResolveCreateProductAsync(Guid userId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_options.Address))
            return new CreateProductStorePolicyDecision { Action = CreateProductStorePolicyAction.StoreUnavailable };

        try
        {
            var response = await _client.ResolveCreateProductAsync(
                new ResolveCreateProductRequest
                {
                    UserId = userId.ToString()
                },
                cancellationToken: cancellationToken);

            if (response.Status.IsFailure())
                return new CreateProductStorePolicyDecision { Action = CreateProductStorePolicyAction.StoreUnavailable };

            if (!Guid.TryParse(response.Data.StoreId, out var storeId))
                storeId = Guid.Empty;

            return new CreateProductStorePolicyDecision
            {
                Action = response.Data.Action.ToDomain(),
                StoreId = storeId
            };
        }
        catch (RpcException)
        {
            return new CreateProductStorePolicyDecision { Action = CreateProductStorePolicyAction.StoreUnavailable };
        }
    }

    public async Task<ManageStoreProductPolicyDecision> ResolveManageProductAsync(Guid userId, Guid storeId, string? creatorUserId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_options.Address))
            return new ManageStoreProductPolicyDecision { Action = ManageStoreProductPolicyAction.StoreUnavailable };

        try
        {
            var response = await _client.ResolveManageProductAsync(
                new ResolveManageProductRequest
                {
                    UserId = userId.ToString(),
                    StoreId = storeId.ToString(),
                    CreatorUserId = creatorUserId ?? string.Empty
                },
                cancellationToken: cancellationToken);

            if (response.Status.IsFailure())
                return new ManageStoreProductPolicyDecision { Action = ManageStoreProductPolicyAction.StoreUnavailable };

            return new ManageStoreProductPolicyDecision
            {
                Action = response.Data.Action.ToDomain()
            };
        }
        catch (RpcException)
        {
            return new ManageStoreProductPolicyDecision { Action = ManageStoreProductPolicyAction.StoreUnavailable };
        }
    }

    public async Task<OwnerStoreApprovalPolicyDecision> ResolveOwnerStoreApprovalAsync(Guid userId, Guid storeId, string? creatorUserId, ProductStatus productStatus, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_options.Address))
            return new OwnerStoreApprovalPolicyDecision { Action = OwnerStoreApprovalPolicyAction.ForbiddenStoreMembership };

        try
        {
            var response = await _client.ResolveOwnerApprovalAsync(
                new ResolveOwnerApprovalRequest
                {
                    UserId = userId.ToString(),
                    StoreId = storeId.ToString(),
                    CreatorUserId = creatorUserId ?? string.Empty,
                    ProductStatus = productStatus.ToGrpc()
                },
                cancellationToken: cancellationToken);

            if (response.Status.IsFailure())
                return new OwnerStoreApprovalPolicyDecision { Action = OwnerStoreApprovalPolicyAction.ForbiddenStoreMembership };

            return new OwnerStoreApprovalPolicyDecision
            {
                Action = response.Data.Action.ToDomain()
            };
        }
        catch (RpcException)
        {
            return new OwnerStoreApprovalPolicyDecision { Action = OwnerStoreApprovalPolicyAction.ForbiddenStoreMembership };
        }
    }
}

file static class StorePolicyGrpcMappings
{
    public static CreateProductStorePolicyAction ToDomain(this CreateProductPolicyAction action)
    {
        return (int)action switch
        {
            1 => CreateProductStorePolicyAction.Allowed,
            2 => CreateProductStorePolicyAction.StoreUnavailable,
            3 => CreateProductStorePolicyAction.OwnerRequiredForUnverifiedStore,
            _ => CreateProductStorePolicyAction.StoreUnavailable
        };
    }

    public static ManageStoreProductPolicyAction ToDomain(this ManageProductPolicyAction action)
    {
        return (int)action switch
        {
            1 => ManageStoreProductPolicyAction.Allowed,
            2 => ManageStoreProductPolicyAction.StoreUnavailable,
            3 => ManageStoreProductPolicyAction.CreatorInvalid,
            4 => ManageStoreProductPolicyAction.Forbidden,
            _ => ManageStoreProductPolicyAction.StoreUnavailable
        };
    }

    public static OwnerStoreApprovalPolicyAction ToDomain(this OwnerApprovalPolicyAction action)
    {
        return (int)action switch
        {
            1 => OwnerStoreApprovalPolicyAction.PublishActive,
            2 => OwnerStoreApprovalPolicyAction.MovePendingOwnerReview,
            3 => OwnerStoreApprovalPolicyAction.MovePendingAdminReview,
            4 => OwnerStoreApprovalPolicyAction.UseAdminApproval,
            5 => OwnerStoreApprovalPolicyAction.ForbiddenStoreMembership,
            6 => OwnerStoreApprovalPolicyAction.ForbiddenCreatorOnly,
            7 => OwnerStoreApprovalPolicyAction.ForbiddenOwnerOnlyUnverified,
            8 => OwnerStoreApprovalPolicyAction.ForbiddenOwnerOnlyApprove,
            9 => OwnerStoreApprovalPolicyAction.CreatorInvalid,
            10 => OwnerStoreApprovalPolicyAction.NotReady,
            _ => OwnerStoreApprovalPolicyAction.ForbiddenStoreMembership
        };
    }

    public static ProductApprovalStatus ToGrpc(this ProductStatus status)
    {
        return status switch
        {
            ProductStatus.Draft => (ProductApprovalStatus)1,
            ProductStatus.PendingOwnerReview => (ProductApprovalStatus)2,
            ProductStatus.PendingAdminReview => (ProductApprovalStatus)3,
            ProductStatus.Active => (ProductApprovalStatus)4,
            ProductStatus.Inactive => (ProductApprovalStatus)5,
            ProductStatus.Deleted => (ProductApprovalStatus)6,
            _ => (ProductApprovalStatus)0
        };
    }
}
