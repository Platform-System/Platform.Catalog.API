using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Platform.Catalog.API.Infrastructure.Persistence.Models;

namespace Platform.Catalog.API.Infrastructure.Persistence.Configurations;

public sealed class ProductCoverImageConfiguration : IEntityTypeConfiguration<ProductCoverImageModel>
{
    public void Configure(EntityTypeBuilder<ProductCoverImageModel> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.BlobName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(x => x.ContainerName)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.FileName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(x => x.ContentType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Size)
            .IsRequired();

        builder.Property(x => x.Url);

        builder.HasIndex(x => x.ProductId)
            .IsUnique();
    }
}
