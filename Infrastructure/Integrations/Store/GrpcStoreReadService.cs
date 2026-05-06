using Grpc.Core;
using Microsoft.Extensions.Options;
using Platform.Catalog.API.Application.Abstractions.Stores;
using Platform.Common.Grpc;
using Platform.StoreService.Grpc;

namespace Platform.Catalog.API.Infrastructure.Integrations.Store;

public sealed class GrpcStoreReadService : IStoreReadService
{
    private readonly StoreIntegration.StoreIntegrationClient _client;
    private readonly StoreClientOptions _options;

    public GrpcStoreReadService(StoreIntegration.StoreIntegrationClient client, IOptions<StoreClientOptions> options)
    {
        _client = client;
        _options = options.Value;
    }

    public async Task<Guid?> GetCurrentOwnerStoreIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_options.Address))
            return null;

        try
        {
            var response = await _client.GetCurrentOwnerStoreAsync(
                new GetCurrentOwnerStoreRequest
                {
                    UserId = userId.ToString()
                },
                cancellationToken: cancellationToken);

            if (response.Status.IsFailure())
                return null;

            return Guid.TryParse(response.Data.StoreId, out var storeId) ? storeId : null;
        }
        catch (RpcException)
        {
            return null;
        }
    }

    public async Task<Guid?> GetStoreIdBySlugAsync(string slug, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_options.Address))
            return null;

        try
        {
            var response = await _client.GetStoreBySlugAsync(
                new GetStoreBySlugRequest
                {
                    Slug = slug
                },
                cancellationToken: cancellationToken);

            if (response.Status.IsFailure())
                return null;

            return Guid.TryParse(response.Data.StoreId, out var storeId) ? storeId : null;
        }
        catch (RpcException)
        {
            return null;
        }
    }
}
