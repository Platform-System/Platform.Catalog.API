using Platform.Application.Messaging;

namespace Platform.Catalog.API.Application.Features.Products.Commands.AdminApproveProduct;

public sealed class AdminApproveProductCommand : ICommand
{
    public Guid ProductId { get; }

    public AdminApproveProductCommand(Guid productId)
    {
        ProductId = productId;
    }
}
