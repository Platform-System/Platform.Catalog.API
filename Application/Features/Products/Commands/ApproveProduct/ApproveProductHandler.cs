using Platform.Application.Abstractions.Data;
using Platform.Application.Abstractions.Storage;
using Platform.Application.Messaging;
using Microsoft.AspNetCore.Http;
using Platform.BuildingBlocks.Responses;
using Platform.Catalog.API.Application.Features.Products.Mappers;
using Platform.Catalog.API.Domain.Enums;
using Platform.Catalog.API.Infrastructure.Persistence.Models;
using MediatR;

namespace Platform.Catalog.API.Application.Features.Products.Commands.ApproveProduct;

public sealed class ApproveProductHandler : ICommandHandler<ApproveProductCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBlobService _blobService;

    public ApproveProductHandler(IUnitOfWork unitOfWork, IBlobService blobService)
    {
        _unitOfWork = unitOfWork;
        _blobService = blobService;
    }

    public async Task<Result<Unit>> Handle(ApproveProductCommand command, CancellationToken cancellationToken)
    {
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
            return Result<Unit>.Failure(StatusCodes.Status404NotFound, "Product not found");

        if (productModel.Status == ProductStatus.Active)
            return Result<Unit>.Failure(StatusCodes.Status400BadRequest, "Product already approved");

        var product = productModel.ToDomain();
        var blob = product.GetBlob();

        if (blob is null)
            return Result<Unit>.Failure(StatusCodes.Status404NotFound, "Blob not found");

        var publicUrl = await _blobService.MakePublicAndGetUrl(blob.ContainerName, blob.BlobName);

        var publishResult = product.PublishBlob(publicUrl);
        if (publishResult.IsFailure)
            return Result<Unit>.Failure(StatusCodes.Status400BadRequest, "Unable to publish product blob.");

        var activateResult = product.Activate();
        if (activateResult.IsFailure)
            return Result<Unit>.Failure(StatusCodes.Status400BadRequest, "Unable to approve product.");

        if (product.CoverImage is not null)
        {
            var coverImageUrl = await _blobService.MakePublicAndGetUrl(product.CoverImage.ContainerName, product.CoverImage.BlobName);
            product.CoverImage.Publish(coverImageUrl);
        }

        productModel.ApplyDomainState(product);
        if (productModel.CoverImage is not null && product.CoverImage is not null)
        {
            productModel.CoverImage.ApplyDomainState(product.CoverImage);
        }

        _unitOfWork.GetRepository<ProductModel>().Update(productModel);

        return Result<Unit>.Success(Unit.Value);
    }
}
