using Platform.Application.Messaging;
using Platform.Catalog.API.Application.Features.Products.Shared;

namespace Platform.Catalog.API.Application.Features.Products.Commands.Create;

public sealed class CreateProductCommand : ICommand<ProductResponse>
{
    public CreateProductRequest Request { get; }
    public ProductImageRequest Image { get; }

    public CreateProductCommand(CreateProductRequest request, ProductImageRequest image)
    {
        Request = request;
        Image = image;
    }
}
