using Platform.Catalog.API.Domain.Entities;
using Platform.Catalog.API.Infrastructure.Persistence.Models;

namespace Platform.Catalog.API.Application.Features.Stores.Mappers;

public static class StorePersistenceMapper
{
    public static Store ToDomain(this StoreModel store)
    {
        return Store.Load(
            store.Id,
            store.Name,
            store.Slug,
            store.Description,
            store.Tagline,
            store.Location,
            store.ResponseTime,
            store.AvatarUrl,
            store.CoverImageUrl,
            store.IsVerified,
            store.Status,
            store.ShippingPolicy,
            store.ReturnPolicy,
            store.WarrantyPolicy);
    }

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

    public static void ApplyDomainState(this StoreModel model, Store store)
    {
        model.Name = store.Name;
        model.Slug = store.Slug;
        model.Description = store.Description;
        model.Tagline = store.Tagline;
        model.Location = store.Location;
        model.ResponseTime = store.ResponseTime;
        model.AvatarUrl = store.AvatarUrl;
        model.CoverImageUrl = store.CoverImageUrl;
        model.IsVerified = store.IsVerified;
        model.Status = store.Status;
        model.ShippingPolicy = store.ShippingPolicy;
        model.ReturnPolicy = store.ReturnPolicy;
        model.WarrantyPolicy = store.WarrantyPolicy;
    }
}
