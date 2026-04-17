namespace Platform.Catalog.API.Application.Features.Products.Commands.UploadImage;

public static class UploadImageMapper
{
    public static UploadImageResponse ToResponse(this (string BlobName, string ContainerName) data)
    {
        return new UploadImageResponse
        {
            BlobName = data.BlobName,
            ContainerName = data.ContainerName
        };
    }
}
