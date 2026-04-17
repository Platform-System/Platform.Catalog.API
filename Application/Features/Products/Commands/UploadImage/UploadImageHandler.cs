using Platform.Application.Messaging;
using Platform.Application.Abstractions.Storage;
using Platform.BuildingBlocks.Responses;

namespace Platform.Catalog.API.Application.Features.Products.Commands.UploadImage;

public sealed class UploadImageHandler : ICommandHandler<UploadImageCommand, UploadImageResponse>
{
    private readonly IBlobService _blobService;

    public UploadImageHandler(IBlobService blobService)
    {
        _blobService = blobService;
    }

    public async Task<Result<UploadImageResponse>> Handle(UploadImageCommand command, CancellationToken cancellationToken)
    {
        var uploadResult = await _blobService.UploadAsync(
            command.Request.Stream,
            command.Request.FileName,
            command.Request.ContentType,
            cancellationToken);

        return Result<UploadImageResponse>.Success(uploadResult.ToResponse());
    }
}
