using Platform.Catalog.API.Domain.Entities;
using Platform.Catalog.API.Infrastructure.Persistence.Models;

namespace Platform.Catalog.API.Application.Features.StoreMembers.Mappers;

public static class StoreMemberPersistenceMapper
{
    public static StoreMember ToDomain(this StoreMemberModel member)
    {
        return StoreMember.Load(
            member.Id,
            member.StoreId,
            member.UserId,
            member.Role,
            member.Status,
            member.CanPublishProductDirectly,
            member.JoinedAt);
    }

    public static StoreMemberModel ToPersistence(this StoreMember member)
    {
        return new StoreMemberModel
        {
            Id = member.Id,
            StoreId = member.StoreId,
            UserId = member.UserId,
            Role = member.Role,
            Status = member.Status,
            CanPublishProductDirectly = member.CanPublishProductDirectly,
            JoinedAt = member.JoinedAt
        };
    }

    public static void ApplyDomainState(this StoreMemberModel model, StoreMember member)
    {
        model.Role = member.Role;
        model.Status = member.Status;
        model.CanPublishProductDirectly = member.CanPublishProductDirectly;
        model.JoinedAt = member.JoinedAt;
    }
}
