using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Platform.Catalog.API.Infrastructure.Persistence.Models;

namespace Platform.Catalog.API.Infrastructure.Persistence.Configurations;

public sealed class StoreMemberConfiguration : IEntityTypeConfiguration<StoreMemberModel>
{
    public void Configure(EntityTypeBuilder<StoreMemberModel> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.StoreId)
            .IsRequired();

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.Property(x => x.Role)
            .IsRequired();

        builder.Property(x => x.Status)
            .IsRequired();

        builder.Property(x => x.CanPublishProductDirectly)
            .IsRequired();

        builder.Property(x => x.JoinedAt)
            .IsRequired();

        builder.HasIndex(x => new { x.StoreId, x.UserId })
            .IsUnique();

        builder.HasIndex(x => x.UserId)
            .IsUnique();
    }
}
