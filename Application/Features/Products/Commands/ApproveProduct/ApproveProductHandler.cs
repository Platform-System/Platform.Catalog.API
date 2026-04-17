using Microsoft.EntityFrameworkCore;
using Platform.Application.Abstractions.Data;
using Platform.Application.Abstractions.Storage;
using Platform.Application.Messaging;
using Platform.BuildingBlocks.Responses;
using Platform.Catalog.API.Application.Mappers;
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
            .GetQueryable()
            .Include(x => x.ProductTypes)
            .Include(x => x.MediaFiles)
            .FirstOrDefaultAsync(x => x.Id == command.ProductId, cancellationToken);

        if (productModel is null || productModel.Status == ProductStatus.Deleted)
            return Result<Unit>.Failure("Product not found");

        if (productModel.Status == ProductStatus.Active)
            return Result<Unit>.Failure("Product already approved");

        var product = productModel.ToDomain();
        var blob = product.GetBlob();

        if (blob is null)
            return Result<Unit>.Failure("Blob not found");

        var publicUrl = await _blobService.MakePublicAndGetUrl(blob.ContainerName, blob.BlobName);

        var publishResult = product.PublishBlob(publicUrl);
        if (publishResult.IsFailure)
            return Result<Unit>.Failure("Unable to publish product blob.");

        var activateResult = product.Activate();
        if (activateResult.IsFailure)
            return Result<Unit>.Failure("Unable to approve product.");

        productModel.ApplyDomainState(product);
        _unitOfWork.GetRepository<ProductModel>().Update(productModel);

        return Result<Unit>.Success(Unit.Value);
    }
}
