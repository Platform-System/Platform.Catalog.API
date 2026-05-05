using Platform.Application.Messaging;

namespace Platform.Catalog.API.Application.Features.Stores.Commands.RequestVerification;

public sealed class RequestStoreVerificationCommand : ICommand
{
    public Guid StoreId { get; }

    public RequestStoreVerificationCommand(Guid storeId)
    {
        StoreId = storeId;
    }
}
