using Platform.Application.Messaging;
using Platform.Catalog.API.Application.Features.Products.Shared;

namespace Platform.Catalog.API.Application.Features.Products.Commands.Update;

public sealed class UpdateProductCommand : ICommand<ProductResponse>
{
    public Guid ProductId { get; }
    public UpdateProductRequest Request { get; }
    public ProductImageRequest Image { get; }

    public UpdateProductCommand(Guid productId, UpdateProductRequest request, ProductImageRequest image)
    {
        ProductId = productId;
        Request = request;
        Image = image;
    }
}
