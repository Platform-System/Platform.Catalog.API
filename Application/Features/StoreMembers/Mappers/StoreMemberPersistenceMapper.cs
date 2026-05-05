using Platform.Catalog.API.Domain.Entities;
using Platform.Catalog.API.Infrastructure.Persistence.Models;

namespace Platform.Catalog.API.Application.Features.StoreMembers.Mappers;

public static class StoreMemberPersistenceMapper
{
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
}
