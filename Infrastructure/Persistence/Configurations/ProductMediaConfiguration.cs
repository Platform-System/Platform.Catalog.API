using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Platform.Catalog.API.Infrastructure.Persistence.Models;

namespace Platform.Catalog.API.Infrastructure.Persistence.Configurations
{
    public class ProductMediaConfiguration : IEntityTypeConfiguration<ProductMediaModel>
    {
        public void Configure(EntityTypeBuilder<ProductMediaModel> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.FileName).IsRequired().HasMaxLength(255);
            builder.Property(x => x.Url).IsRequired();
            builder.Property(x => x.ContentType).HasMaxLength(50);
        }
    }
}
