namespace Platform.Catalog.API.Application.Features.StoreMembers.Commands.UpdatePublishPermission;

public sealed class UpdatePublishPermissionRequest
{
    public bool CanPublishProductDirectly { get; init; }
}
