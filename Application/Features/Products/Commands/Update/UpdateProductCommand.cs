using Platform.Application.Messaging;
using Platform.Catalog.API.Application.Features.Products.Shared;

namespace Platform.Catalog.API.Application.Features.Products.Commands.Update;

public sealed class UpdateProductCommand : ICommand<ProductResponse>
{
    public Guid ProductId { get; }
    public string BlobName { get; }
    public string ContainerName { get; }
    public UpdateProductRequest Request { get; }

    public UpdateProductCommand(Guid productId, string blobName, string containerName, UpdateProductRequest request)
    {
        ProductId = productId;
        BlobName = blobName;
        ContainerName = containerName;
        Request = request;
    }
}
