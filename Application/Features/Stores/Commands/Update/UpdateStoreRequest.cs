namespace Platform.Catalog.API.Application.Features.Stores.Commands.Update;

public sealed class UpdateStoreRequest
{
    public string Name { get; init; } = null!;
    public string? Description { get; init; }
    public string? Tagline { get; init; }
    public string? Location { get; init; }
    public string? ResponseTime { get; init; }
    public string? AvatarUrl { get; init; }
    public string? CoverImageUrl { get; init; }
    public string? ShippingPolicy { get; init; }
    public string? ReturnPolicy { get; init; }
    public string? WarrantyPolicy { get; init; }
}
