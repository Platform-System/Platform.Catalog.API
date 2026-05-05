using Microsoft.EntityFrameworkCore;
using Platform.BuildingBlocks.Abstractions;
using Platform.Catalog.API.Infrastructure.Persistence.Models;
using Platform.Infrastructure.Data;
using System.Reflection;

namespace Platform.Catalog.API.Infrastructure.Data
{
    public class CatalogDbContext : BaseDbContext
    {
        public CatalogDbContext(DbContextOptions<CatalogDbContext> options, ICurrentUserProvider? currentUserProvider = null)
            : base(options, currentUserProvider)
        {
        }
        public DbSet<ProductModel> Products { get; set; }
        public DbSet<CategoryModel> Categories { get; set; }
        public DbSet<StoreModel> Stores { get; set; }
        public DbSet<StoreMemberModel> StoreMembers { get; set; }
        public DbSet<ProductMediaModel> ProductMedias { get; set; }
        public DbSet<ProductCoverImageModel> ProductCoverImages { get; set; }
        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Properties<Enum>()
                .HaveConversion<string>();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
