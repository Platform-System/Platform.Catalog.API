using Platform.Application.Messaging;
using Platform.Catalog.API.Application.Features.Products.Responses;

namespace Platform.Catalog.API.Application.Features.Products.Commands.Delete;

public sealed class DeleteProductCommand : ICommand<ProductResponse>
{
    public Guid ProductId { get; }

    public DeleteProductCommand(Guid productId)
    {
        ProductId = productId;
    }
}
