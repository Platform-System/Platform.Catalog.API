using Platform.Catalog.API.Domain.Enums;
using Platform.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace Platform.Catalog.API.Infrastructure.Persistence.Models;

[Table("StoreMembers")]
public sealed class StoreMemberModel : Entity
{
    public Guid StoreId { get; set; }
    public Guid UserId { get; set; }
    public StoreMemberRole Role { get; set; }
    public StoreMemberStatus Status { get; set; }
    public bool CanPublishProductDirectly { get; set; }
    public DateTime JoinedAt { get; set; }

    public StoreModel Store { get; set; } = null!;
}
