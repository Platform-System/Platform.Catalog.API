using Platform.BuildingBlocks.DateTimes;
using Platform.Catalog.API.Domain.Enums;
using Platform.Domain.Common;

namespace Platform.Catalog.API.Domain.Entities;

public sealed class StoreMember : Entity
{
    public Guid StoreId { get; private set; }
    public Guid UserId { get; private set; }
    public StoreMemberRole Role { get; private set; }
    public StoreMemberStatus Status { get; private set; }
    public bool CanPublishProductDirectly { get; private set; }
    public DateTime JoinedAt { get; private set; }

    private StoreMember()
    {
    }

    public static StoreMember Create(Guid storeId, Guid userId, StoreMemberRole role)
    {
        return new StoreMember
        {
            StoreId = storeId,
            UserId = userId,
            Role = role,
            Status = StoreMemberStatus.Active,
            CanPublishProductDirectly = role == StoreMemberRole.Owner,
            JoinedAt = Clock.Now
        };
    }

    public static StoreMember Invite(Guid storeId, Guid userId, StoreMemberRole role, bool canPublishProductDirectly)
    {
        return new StoreMember
        {
            StoreId = storeId,
            UserId = userId,
            Role = role,
            Status = StoreMemberStatus.Invited,
            CanPublishProductDirectly = canPublishProductDirectly,
            JoinedAt = Clock.Now
        };
    }

    public static StoreMember Load(Guid id, Guid storeId, Guid userId, StoreMemberRole role, StoreMemberStatus status, bool canPublishProductDirectly, DateTime joinedAt)
    {
        return new StoreMember
        {
            Id = id,
            StoreId = storeId,
            UserId = userId,
            Role = role,
            Status = status,
            CanPublishProductDirectly = canPublishProductDirectly,
            JoinedAt = joinedAt
        };
    }

    public DomainResult AcceptInvite()
    {
        if (Status != StoreMemberStatus.Invited)
            return DomainResult.Failure(DomainErrors.Validation.InvalidInput);

        Status = StoreMemberStatus.Active;
        return DomainResult.Success();
    }
}
