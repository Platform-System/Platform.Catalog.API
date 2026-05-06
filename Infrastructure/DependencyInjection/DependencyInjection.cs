using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Platform.Catalog.API.Application.Abstractions.Stores;
using Platform.Catalog.API.Application.Features.Products.Services;
using Platform.Catalog.API.Infrastructure.Integrations.Store;
using Platform.Catalog.API.Infrastructure.Data;
using Platform.Infrastructure.DependencyInjection;
using Platform.Infrastructure.Data;
using Platform.StoreService.Grpc;
using Platform.SystemContext.DependencyInjection;
using StackExchange.Redis;

namespace Platform.Catalog.API.Infrastructure.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddCatalogInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("CatalogDb");
        var redisConnectionString = configuration.GetConnectionString("Redis")
            ?? "localhost:6379,abortConnect=false";

        services.AddSystemContext();
        services.AddInfrastructure(configuration);

        services.AddDbContext<CatalogDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
        });

        services.AddScoped<BaseDbContext>(sp => sp.GetRequiredService<CatalogDbContext>());
        services.AddScoped<ProductApprovalService>();
        services.Configure<StoreClientOptions>(configuration.GetSection("Integrations:Store"));
        services.AddGrpcClient<StoreIntegration.StoreIntegrationClient>((sp, options) =>
        {
            var storeOptions = sp.GetRequiredService<IOptions<StoreClientOptions>>().Value;
            options.Address = new Uri(
                string.IsNullOrWhiteSpace(storeOptions.Address)
                    ? "http://localhost"
                    : storeOptions.Address);
        });
        services.AddScoped<IStoreReadService, GrpcStoreReadService>();
        services.AddScoped<IStorePolicyService, GrpcStorePolicyService>();
        services.AddSingleton<IConnectionMultiplexer>(_ =>
            ConnectionMultiplexer.Connect(redisConnectionString));

        return services;
    }
}
