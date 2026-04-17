using Platform.Application.Messaging;

namespace Platform.Catalog.API.Application.Features.Products.Commands.UploadImage;

public sealed class UploadImageCommand : ICommand<UploadImageResponse>
{
    public UploadImageRequest Request { get; }

    public UploadImageCommand(UploadImageRequest request)
    {
        Request = request;
    }
}
