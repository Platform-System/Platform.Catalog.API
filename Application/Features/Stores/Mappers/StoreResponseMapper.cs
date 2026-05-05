using Platform.Catalog.API.Application.Features.Stores.Shared;
using Platform.Catalog.API.Domain.Entities;

namespace Platform.Catalog.API.Application.Features.Stores.Mappers;

public static class StoreResponseMapper
{
    public static StoreResponse ToResponse(this Store store)
    {
        return new StoreResponse
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
            Status = store.Status.ToString(),
            ShippingPolicy = store.ShippingPolicy,
            ReturnPolicy = store.ReturnPolicy,
            WarrantyPolicy = store.WarrantyPolicy
        };
    }
}
