using Platform.Application.Messaging;

namespace Platform.Catalog.API.Application.Features.StoreMembers.Commands.AcceptInvite;

public sealed class AcceptStoreInviteCommand : ICommand
{
    public Guid StoreId { get; }

    public AcceptStoreInviteCommand(Guid storeId)
    {
        StoreId = storeId;
    }
}
