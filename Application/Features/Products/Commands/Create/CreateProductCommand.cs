using Platform.Application.Messaging;
using Platform.Catalog.API.Application.Features.Products.Shared;

namespace Platform.Catalog.API.Application.Features.Products.Commands.Create;

public sealed class CreateProductCommand : ICommand<ProductResponse>
{
    public string BlobName { get; }
    public string ContainerName { get; }
    public CreateProductRequest Request { get; }

    public CreateProductCommand(string blobName, string containerName, CreateProductRequest request)
    {
        BlobName = blobName;
        ContainerName = containerName;
        Request = request;
    }
}
