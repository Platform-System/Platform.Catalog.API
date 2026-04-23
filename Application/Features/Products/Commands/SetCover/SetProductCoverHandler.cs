using Microsoft.EntityFrameworkCore;
using Platform.Application.Abstractions.Data;
using Platform.Application.Abstractions.Storage;
using Platform.Application.Messaging;
using Platform.BuildingBlocks.Abstractions;
using Platform.BuildingBlocks.Responses;
using Platform.Catalog.API.Application.Features.Products.Shared;
using Platform.Catalog.API.Application.Mappers;
using Platform.Catalog.API.Domain.Enums;
using Platform.Catalog.API.Infrastructure.Persistence.Models;

namespace Platform.Catalog.API.Application.Features.Products.Commands.SetCover;

public sealed class SetProductCoverHandler : ICommandHandler<SetProductCoverCommand, ProductResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IBlobService _blobService;

    public SetProductCoverHandler(IUnitOfWork unitOfWork, ICurrentUserProvider currentUserProvider, IBlobService blobService)
    {
        _unitOfWork = unitOfWork;
        _currentUserProvider = currentUserProvider;
        _blobService = blobService;
    }

    public async Task<Result<ProductResponse>> Handle(SetProductCoverCommand command, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(_currentUserProvider.CurrentUserId, out var currentUserId))
            return Result<ProductResponse>.Failure("Current user is invalid.");

        var productModel = await _unitOfWork
            .GetRepository<ProductModel>()
            .GetQueryable()
            .Include(x => x.ProductTypes)
            .Include(x => x.MediaFiles)
            .Include(x => x.CoverImage)
            .FirstOrDefaultAsync(x => x.Id == command.ProductId, cancellationToken);

        if (productModel is null || productModel.Status == ProductStatus.Deleted)
            return Result<ProductResponse>.Failure("Product not found.");

        if (productModel.Status == ProductStatus.Active)
            return Result<ProductResponse>.Failure("Active product cover cannot be updated.");

        if (!Guid.TryParse(productModel.CreatedBy, out var ownerId) || ownerId != currentUserId)
            return Result<ProductResponse>.Failure("You do not own this product.");

        var product = productModel.ToDomain();
        var coverImage = command.Request.ToCoverImage(product);
        product.SetCoverImage(coverImage);

        if (productModel.CoverImage is null)
        {
            var coverImageModel = coverImage.ToPersistence();
            productModel.CoverImage = coverImageModel;
            await _unitOfWork.GetRepository<ProductCoverImageModel>().AddAsync(coverImageModel, cancellationToken);
        }
        else
        {
            productModel.CoverImage.ApplyDomainState(coverImage);
            _unitOfWork.GetRepository<ProductCoverImageModel>().Update(productModel.CoverImage);
        }

        return Result<ProductResponse>.Success(productModel.ToResponse(_blobService));
    }
}
