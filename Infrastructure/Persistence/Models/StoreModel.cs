using Platform.Catalog.API.Domain.Enums;
using Platform.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace Platform.Catalog.API.Infrastructure.Persistence.Models;

[Table("Stores")]
public sealed class StoreModel : Entity
{
    public string Name { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public string? Description { get; set; }
    public string? Tagline { get; set; }
    public string? Location { get; set; }
    public string? ResponseTime { get; set; }
    public string? AvatarUrl { get; set; }
    public string? CoverImageUrl { get; set; }
    public bool IsVerified { get; set; }
    public StoreStatus Status { get; set; }
    public string? ShippingPolicy { get; set; }
    public string? ReturnPolicy { get; set; }
    public string? WarrantyPolicy { get; set; }

    public List<StoreMemberModel> Members { get; set; } = [];
    public List<ProductModel> Products { get; set; } = [];
}
