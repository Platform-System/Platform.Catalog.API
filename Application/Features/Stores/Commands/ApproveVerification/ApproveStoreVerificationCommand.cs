using Platform.Application.Messaging;

namespace Platform.Catalog.API.Application.Features.Stores.Commands.ApproveVerification;

public sealed class ApproveStoreVerificationCommand : ICommand
{
    public Guid StoreId { get; }

    public ApproveStoreVerificationCommand(Guid storeId)
    {
        StoreId = storeId;
    }
}
