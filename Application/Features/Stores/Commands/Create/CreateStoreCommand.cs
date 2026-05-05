using Platform.Application.Messaging;
using Platform.Catalog.API.Application.Features.Stores.Shared;

namespace Platform.Catalog.API.Application.Features.Stores.Commands.Create;

public sealed class CreateStoreCommand : ICommand<StoreResponse>
{
    public CreateStoreRequest Request { get; }

    public CreateStoreCommand(CreateStoreRequest request)
    {
        Request = request;
    }
}
