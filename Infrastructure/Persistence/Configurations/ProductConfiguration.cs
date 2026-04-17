using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Platform.Catalog.API.Infrastructure.Persistence.Models;

namespace Platform.Catalog.API.Infrastructure.Persistence.Configurations
{
    public sealed class ProductConfiguration : IEntityTypeConfiguration<ProductModel>
    {
        public void Configure(EntityTypeBuilder<ProductModel> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasDiscriminator<string>("ProductKind")
                .HasValue<DigitalProductModel>("Digital")
                .HasValue<PhysicalProductModel>("Physical");

            builder.Property(x => x.Title).IsRequired().HasMaxLength(250);
            builder.Property(x => x.Author).IsRequired().HasMaxLength(150);
            builder.Property(x => x.Price).IsRequired();

            builder.Property(x => x.AdditionalInfo)
                .HasColumnType("jsonb");

            builder.HasMany(x => x.ProductTypes)
                .WithMany(x => x.Products);

            builder.HasMany(x => x.MediaFiles)
                .WithOne(x => x.Product)
                .HasForeignKey(x => x.ProductId);
        }
    }
}
