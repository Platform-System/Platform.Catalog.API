using Platform.Application.Messaging;

namespace Platform.Catalog.API.Application.Features.Products.Commands.ApproveProduct;

public sealed class ApproveProductCommand : ICommand
{
    public Guid ProductId { get; }

    public ApproveProductCommand(Guid productId)
    {
        ProductId = productId;
    }
}
