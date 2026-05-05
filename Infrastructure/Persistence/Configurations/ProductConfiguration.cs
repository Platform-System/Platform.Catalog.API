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

            builder.Property(x => x.Title).IsRequired().HasMaxLength(250);
            builder.Property(x => x.Author).IsRequired().HasMaxLength(150);
            builder.Property(x => x.Price).IsRequired();
            builder.Property(x => x.Stock).IsRequired();
            builder.Property(x => x.CategoryId).IsRequired();
            builder.Property(x => x.StoreId).IsRequired();

            builder.Property(x => x.AdditionalInfo)
                .HasColumnType("jsonb");

            builder.HasOne(x => x.Category)
                .WithMany(x => x.Products)
                .HasForeignKey(x => x.CategoryId);

            builder.HasOne(x => x.Store)
                .WithMany(x => x.Products)
                .HasForeignKey(x => x.StoreId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.MediaFiles)
                .WithOne(x => x.Product)
                .HasForeignKey(x => x.ProductId);

            builder.HasOne(x => x.CoverImage)
                .WithOne(x => x.Product)
                .HasForeignKey<ProductCoverImageModel>(x => x.ProductId);
        }
    }
}
