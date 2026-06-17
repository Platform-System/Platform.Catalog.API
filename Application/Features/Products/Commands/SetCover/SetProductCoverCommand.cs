using Platform.Application.Messaging;

namespace Platform.Catalog.API.Application.Features.Products.Commands.SetCover;

public sealed class SetProductCoverCommand : ICommand<bool>
{
    public Guid ProductId { get; }
    public SetProductCoverRequest Request { get; }

    public SetProductCoverCommand(Guid productId, SetProductCoverRequest request)
    {
        ProductId = productId;
        Request = request;
    }
}
