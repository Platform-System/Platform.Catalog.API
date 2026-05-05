using Microsoft.EntityFrameworkCore;
using Platform.Catalog.API.Application.Features.Products.Services;
using Platform.Catalog.API.Infrastructure.Data;
using Platform.Infrastructure.DependencyInjection;
using Platform.Infrastructure.Data;
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
        services.AddSingleton<IConnectionMultiplexer>(_ =>
            ConnectionMultiplexer.Connect(redisConnectionString));

        return services;
    }
}
