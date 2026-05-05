using Platform.Application.Messaging;
using Platform.Catalog.API.Application.Features.Stores.Shared;

namespace Platform.Catalog.API.Application.Features.Stores.Commands.Update;

public sealed class UpdateStoreCommand : ICommand<StoreResponse>
{
    public UpdateStoreRequest Request { get; }

    public UpdateStoreCommand(UpdateStoreRequest request)
    {
        Request = request;
    }
}
