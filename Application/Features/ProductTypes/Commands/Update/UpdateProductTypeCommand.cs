using Platform.Application.Messaging;
using Platform.Catalog.API.Application.Features.ProductTypes.Shared;

namespace Platform.Catalog.API.Application.Features.ProductTypes.Commands.Update;

public sealed class UpdateProductTypeCommand : ICommand<ProductTypeResponse>
{
    public UpdateProductTypeCommand(Guid productTypeId, UpdateProductTypeRequest request)
    {
        ProductTypeId = productTypeId;
        Request = request;
    }

    public Guid ProductTypeId { get; }
    public UpdateProductTypeRequest Request { get; }
}
