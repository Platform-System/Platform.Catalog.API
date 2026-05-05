using Platform.Catalog.API.Domain.Enums;

namespace Platform.Catalog.API.Application.Features.StoreMembers.Commands.InviteMember;

public sealed class InviteStoreMemberRequest
{
    public Guid UserId { get; init; }
    public StoreMemberRole Role { get; init; }
    public bool CanPublishProductDirectly { get; init; }
}
