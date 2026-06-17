using Platform.Application.Abstractions.Data;
using Platform.Application.Messaging;
using Platform.BuildingBlocks.Responses;
using Platform.Catalog.API.Application.Features.Products.Mappers;
using Platform.Catalog.API.Domain.Entities;
using Platform.Catalog.API.Domain.Enums;
using Platform.Catalog.API.Infrastructure.Persistence.Models;
using Platform.SystemContext.Abstractions;

namespace Platform.Catalog.API.Application.Features.Products.Commands.SetCover;

public sealed class SetProductCoverHandler : ICommandHandler<SetProductCoverCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;

    public SetProductCoverHandler(IUnitOfWork unitOfWork, IUserContext userContext)
    {
        _unitOfWork = unitOfWork;
        _userContext = userContext;
    }

    public async Task<Result<bool>> Handle(SetProductCoverCommand command, CancellationToken cancellationToken)
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
                x => x.MediaFiles,
                x => x.CoverImage!);

        if (productModel is null || productModel.Status == ProductStatus.Deleted)
            return Result<bool>.Failure(StatusCodes.Status404NotFound, "Product not found.");

        if (productModel.Status == ProductStatus.Active)
            return Result<bool>.Failure(StatusCodes.Status400BadRequest, "Active product cover cannot be updated.");

        if (!Guid.TryParse(productModel.CreatedBy, out var ownerId) || ownerId != userId)
            return Result<bool>.Failure(StatusCodes.Status403Forbidden, "You do not own this product.");

        var product = productModel.ToDomain();

        ProductCoverImage coverImage;
        if (product.CoverImage is null)
        {
            coverImage = new ProductCoverImage(
                product.Id,
                command.Request.BlobName,
                command.Request.ContainerName,
                command.Request.FileName,
                command.Request.ContentType,
                command.Request.Size,
                command.Request.AltText);
            product.SetCoverImage(coverImage);
            productModel.CoverImage = coverImage.ToPersistence();
            await _unitOfWork.GetRepository<ProductCoverImageModel>().AddAsync(productModel.CoverImage, cancellationToken);
        }
        else
        {
            product.CoverImage.UpdateMetadata(
                command.Request.BlobName,
                command.Request.ContainerName,
                command.Request.FileName,
                command.Request.ContentType,
                command.Request.Size,
                command.Request.AltText);
            coverImage = product.CoverImage;
            product.SetCoverImage(coverImage);
            productModel.CoverImage!.ApplyDomainState(coverImage);
            _unitOfWork.GetRepository<ProductCoverImageModel>().Update(productModel.CoverImage!);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
