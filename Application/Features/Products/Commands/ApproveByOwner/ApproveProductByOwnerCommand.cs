using Platform.Application.Messaging;

namespace Platform.Catalog.API.Application.Features.Products.Commands.ApproveByOwner;

public sealed class ApproveProductByOwnerCommand : ICommand
{
    public Guid ProductId { get; }

    public ApproveProductByOwnerCommand(Guid productId)
    {
        ProductId = productId;
    }
}
