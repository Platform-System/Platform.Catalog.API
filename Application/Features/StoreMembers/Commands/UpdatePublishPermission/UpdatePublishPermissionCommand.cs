using Platform.Application.Messaging;

namespace Platform.Catalog.API.Application.Features.StoreMembers.Commands.UpdatePublishPermission;

public sealed class UpdatePublishPermissionCommand : ICommand
{
    public Guid UserId { get; }
    public UpdatePublishPermissionRequest Request { get; }

    public UpdatePublishPermissionCommand(Guid userId, UpdatePublishPermissionRequest request)
    {
        UserId = userId;
        Request = request;
    }
}
