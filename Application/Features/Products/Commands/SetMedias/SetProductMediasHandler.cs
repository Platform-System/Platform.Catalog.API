using Platform.Application.Abstractions.Data;
using Platform.Application.Messaging;
using Platform.BuildingBlocks.Responses;
using Platform.Catalog.API.Application.Features.Products.Mappers;
using Platform.Catalog.API.Domain.Entities;
using Platform.Catalog.API.Domain.Enums;
using Platform.Catalog.API.Infrastructure.Persistence.Models;
using Platform.SystemContext.Abstractions;

namespace Platform.Catalog.API.Application.Features.Products.Commands.SetMedias;

public sealed class SetProductMediasHandler : ICommandHandler<SetProductMediasCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;

    public SetProductMediasHandler(IUnitOfWork unitOfWork, IUserContext userContext)
    {
        _unitOfWork = unitOfWork;
        _userContext = userContext;
    }

    public async Task<Result<bool>> Handle(SetProductMediasCommand command, CancellationToken cancellationToken)
    {
        if (_userContext.UserId is null)
            return Result<bool>.Failure(StatusCodes.Status401Unauthorized, "Unauthorized.");

        var userId = _userContext.UserId.Value;

        var productModel = await _unitOfWork
            .GetRepository<ProductModel>()
            .FindAsync(
                x => x.Id == command.ProductId,
                false,
                cancellationToken,
                x => x.Category,
                x => x.MediaFiles);

        if (productModel is null || productModel.Status == ProductStatus.Deleted)
            return Result<bool>.Failure(StatusCodes.Status404NotFound, "Product not found.");

        if (productModel.Status == ProductStatus.Active)
            return Result<bool>.Failure(StatusCodes.Status400BadRequest, "Active product medias cannot be updated.");

        if (!Guid.TryParse(productModel.CreatedBy, out var ownerId) || ownerId != userId)
            return Result<bool>.Failure(StatusCodes.Status403Forbidden, "You do not own this product.");

        var product = productModel.ToDomain();
        var newMediaEntities = new List<ProductMedia>();
        var sortOrder = 0;

        foreach (var item in command.Request.Items)
        {
            newMediaEntities.Add(new ProductMedia(
                command.ProductId,
                item.BlobName,
                item.ContainerName,
                item.FileName,
                item.ContentType,
                item.Size,
                MediaType.Image,
                sortOrder++,
                item.AltText));
        }

        product.SetMediaFiles(newMediaEntities);

        foreach (var oldMedia in productModel.MediaFiles.ToList())
        {
            _unitOfWork.GetRepository<ProductMediaModel>().Remove(oldMedia);
        }

        foreach (var media in product.MediaFiles)
        {
            await _unitOfWork.GetRepository<ProductMediaModel>().AddAsync(media.ToPersistence(), cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
