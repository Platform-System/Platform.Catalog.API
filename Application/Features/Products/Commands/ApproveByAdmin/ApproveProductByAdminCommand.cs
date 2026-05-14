using Platform.Application.Messaging;

namespace Platform.Catalog.API.Application.Features.Products.Commands.ApproveByAdmin;

public sealed class ApproveProductByAdminCommand : ICommand
{
    public Guid ProductId { get; }

    public ApproveProductByAdminCommand(Guid productId)
    {
        ProductId = productId;
    }
}
