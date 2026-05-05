using MediatR;
using Platform.Application.Abstractions.Data;
using Platform.Application.Abstractions.Storage;
using Platform.BuildingBlocks.Responses;
using Platform.Catalog.API.Application.Features.Products.Mappers;
using Platform.Catalog.API.Domain.Entities;
using Platform.Catalog.API.Infrastructure.Persistence.Models;

namespace Platform.Catalog.API.Application.Features.Products.Services;

public sealed class ProductApprovalService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBlobService _blobService;

    public ProductApprovalService(IUnitOfWork unitOfWork, IBlobService blobService)
    {
        _unitOfWork = unitOfWork;
        _blobService = blobService;
    }

    public async Task<Result<Unit>> PublishActiveAsync(ProductModel productModel, Product product, CancellationToken cancellationToken)
    {
        var blob = product.GetBlob();
        if (blob is null)
            return Result<Unit>.Failure(StatusCodes.Status404NotFound, "Blob not found.");

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
