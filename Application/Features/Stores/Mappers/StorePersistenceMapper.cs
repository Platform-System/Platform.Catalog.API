using Platform.Catalog.API.Domain.Entities;
using Platform.Catalog.API.Infrastructure.Persistence.Models;

namespace Platform.Catalog.API.Application.Features.Stores.Mappers;

public static class StorePersistenceMapper
{
    public static StoreModel ToPersistence(this Store store)
    {
        return new StoreModel
        {
            Id = store.Id,
            Name = store.Name,
            Slug = store.Slug,
            Description = store.Description,
            Tagline = store.Tagline,
            Location = store.Location,
            ResponseTime = store.ResponseTime,
            AvatarUrl = store.AvatarUrl,
            CoverImageUrl = store.CoverImageUrl,
            IsVerified = store.IsVerified,
            Status = store.Status,
            ShippingPolicy = store.ShippingPolicy,
            ReturnPolicy = store.ReturnPolicy,
            WarrantyPolicy = store.WarrantyPolicy
        };
    }
}
