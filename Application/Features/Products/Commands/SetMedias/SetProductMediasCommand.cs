using Platform.Application.Messaging;

namespace Platform.Catalog.API.Application.Features.Products.Commands.SetMedias;

public sealed class SetProductMediasCommand : ICommand<bool>
{
    public Guid ProductId { get; }
    public SetProductMediasRequest Request { get; }

    public SetProductMediasCommand(Guid productId, SetProductMediasRequest request)
    {
        ProductId = productId;
        Request = request;
    }
}
