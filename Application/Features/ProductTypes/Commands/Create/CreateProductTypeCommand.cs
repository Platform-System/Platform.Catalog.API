using Platform.Application.Messaging;
using Platform.Catalog.API.Application.Features.ProductTypes.Shared;

namespace Platform.Catalog.API.Application.Features.ProductTypes.Commands.Create;

public sealed class CreateProductTypeCommand : ICommand<ProductTypeResponse>
{
    public CreateProductTypeCommand(CreateProductTypeRequest request)
    {
        Request = request;
    }

    public CreateProductTypeRequest Request { get; }
}
