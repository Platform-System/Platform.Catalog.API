using Platform.Application.Messaging;
using Platform.Catalog.API.Application.Features.Products.Shared;

namespace Platform.Catalog.API.Application.Features.Products.Commands.Create;

public sealed class CreateProductCommand : ICommand<ProductResponse>
{
    public CreateProductRequest Request { get; }

    public CreateProductCommand(CreateProductRequest request)
    {
        Request = request;
    }
}
