using Platform.Application.Messaging;
using Platform.Catalog.API.Application.Features.Products.Shared;

namespace Platform.Catalog.API.Application.Features.Products.Commands.SetCover;

public sealed class SetProductCoverCommand : ICommand<ProductResponse>
{
    public Guid ProductId { get; }
    public SetProductCoverRequest Request { get; }

    public SetProductCoverCommand(Guid productId, SetProductCoverRequest request)
    {
        ProductId = productId;
        Request = request;
    }
}
