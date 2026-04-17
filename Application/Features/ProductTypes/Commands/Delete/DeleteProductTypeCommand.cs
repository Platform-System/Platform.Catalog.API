using MediatR;
using Platform.Application.Messaging;

namespace Platform.Catalog.API.Application.Features.ProductTypes.Commands.Delete;

public sealed class DeleteProductTypeCommand : ICommand
{
    public DeleteProductTypeCommand(Guid productTypeId)
    {
        ProductTypeId = productTypeId;
    }

    public Guid ProductTypeId { get; }
}
