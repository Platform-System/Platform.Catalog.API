using Platform.Application.Messaging;

namespace Platform.Catalog.API.Application.Features.Products.Commands.OwnerStoreApproveProduct;

public sealed class OwnerStoreApproveProductCommand : ICommand
{
    public Guid ProductId { get; }

    public OwnerStoreApproveProductCommand(Guid productId)
    {
        ProductId = productId;
    }
}
