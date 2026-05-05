using Platform.Catalog.API.Domain.Enums;
using Platform.Catalog.API.Domain.Errors;
using Platform.Domain.Common;

namespace Platform.Catalog.API.Domain.Entities;

public sealed class Store : AggregateRoot
{
    public string Name { get; private set; } = null!;
    public string Slug { get; private set; } = null!;
    public string? Description { get; private set; }
    public string? Tagline { get; private set; }
    public string? Location { get; private set; }
    public string? ResponseTime { get; private set; }
    public string? AvatarUrl { get; private set; }
    public string? CoverImageUrl { get; private set; }
    public bool IsVerified { get; private set; }
    public StoreStatus Status { get; private set; }
    public string? ShippingPolicy { get; private set; }
    public string? ReturnPolicy { get; private set; }
    public string? WarrantyPolicy { get; private set; }

    private readonly List<StoreMember> _members = [];
    public IReadOnlyCollection<StoreMember> Members => _members.AsReadOnly();

    private Store()
    {
    }

    public static Store Create(string name, string slug, string? description = null, string? tagline = null, string? location = null, string? responseTime = null, string? avatarUrl = null, string? coverImageUrl = null, string? shippingPolicy = null, string? returnPolicy = null, string? warrantyPolicy = null)
    {
        return new Store
        {
            Name = name.Trim(),
            Slug = slug.Trim(),
            Description = description?.Trim(),
            Tagline = tagline?.Trim(),
            Location = location?.Trim(),
            ResponseTime = responseTime?.Trim(),
            AvatarUrl = avatarUrl?.Trim(),
            CoverImageUrl = coverImageUrl?.Trim(),
            ShippingPolicy = shippingPolicy?.Trim(),
            ReturnPolicy = returnPolicy?.Trim(),
            WarrantyPolicy = warrantyPolicy?.Trim(),
            Status = StoreStatus.Draft,
            IsVerified = false
        };
    }

    public static Store Load(Guid id, string name, string slug, string? description, string? tagline, string? location, string? responseTime, string? avatarUrl, string? coverImageUrl, bool isVerified, StoreStatus status, string? shippingPolicy, string? returnPolicy, string? warrantyPolicy)
    {
        return new Store
        {
            Id = id,
            Name = name,
            Slug = slug,
            Description = description,
            Tagline = tagline,
            Location = location,
            ResponseTime = responseTime,
            AvatarUrl = avatarUrl,
            CoverImageUrl = coverImageUrl,
            IsVerified = isVerified,
            Status = status,
            ShippingPolicy = shippingPolicy,
            ReturnPolicy = returnPolicy,
            WarrantyPolicy = warrantyPolicy
        };
    }

    public DomainResult RequestVerification()
    {
        if (IsVerified)
            return DomainResult.Failure(StoreErrors.AlreadyVerified);

        if (Status != StoreStatus.Draft)
            return DomainResult.Failure(StoreErrors.CannotRequestVerification);

        Status = StoreStatus.PendingVerification;
        return DomainResult.Success();
    }

    public DomainResult ApproveVerification()
    {
        if (IsVerified)
            return DomainResult.Failure(StoreErrors.AlreadyVerified);

        if (Status != StoreStatus.PendingVerification)
            return DomainResult.Failure(StoreErrors.VerificationNotRequested);

        IsVerified = true;
        Status = StoreStatus.Active;
        return DomainResult.Success();
    }
}
