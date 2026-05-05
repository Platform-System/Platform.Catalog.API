using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Platform.Catalog.API.Infrastructure.Persistence.Models;

namespace Platform.Catalog.API.Infrastructure.Persistence.Configurations;

public sealed class StoreConfiguration : IEntityTypeConfiguration<StoreModel>
{
    public void Configure(EntityTypeBuilder<StoreModel> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Slug)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Description)
            .HasMaxLength(4000);

        builder.Property(x => x.Tagline)
            .HasMaxLength(250);

        builder.Property(x => x.Location)
            .HasMaxLength(250);

        builder.Property(x => x.ResponseTime)
            .HasMaxLength(150);

        builder.Property(x => x.AvatarUrl)
            .HasMaxLength(1000);

        builder.Property(x => x.CoverImageUrl)
            .HasMaxLength(1000);

        builder.Property(x => x.ShippingPolicy)
            .HasMaxLength(2000);

        builder.Property(x => x.ReturnPolicy)
            .HasMaxLength(2000);

        builder.Property(x => x.WarrantyPolicy)
            .HasMaxLength(2000);

        builder.HasIndex(x => x.Slug)
            .IsUnique();

        builder.HasMany(x => x.Members)
            .WithOne(x => x.Store)
            .HasForeignKey(x => x.StoreId);

        builder.HasMany(x => x.Products)
            .WithOne(x => x.Store)
            .HasForeignKey(x => x.StoreId);
    }
}
