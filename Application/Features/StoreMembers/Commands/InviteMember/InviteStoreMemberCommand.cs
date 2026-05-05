using Platform.Application.Messaging;

namespace Platform.Catalog.API.Application.Features.StoreMembers.Commands.InviteMember;

public sealed class InviteStoreMemberCommand : ICommand
{
    public Guid StoreId { get; }
    public InviteStoreMemberRequest Request { get; }

    public InviteStoreMemberCommand(Guid storeId, InviteStoreMemberRequest request)
    {
        StoreId = storeId;
        Request = request;
    }
}
