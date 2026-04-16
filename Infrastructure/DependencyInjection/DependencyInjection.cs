using Microsoft.EntityFrameworkCore;
using Platform.Catalog.API.Infrastructure.Data;
using Platform.SystemContext.DependencyInjection;

namespace Platform.Catalog.API.Infrastructure.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddCatalogInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("CatalogDb");

        services.AddSystemContext();

        services.AddDbContext<CatalogDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
        });

        return services;
    }
}
