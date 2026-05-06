using Platform.Application.Abstractions.Data;
using Platform.Application.Abstractions.Storage;
using Platform.Application.Messaging;
using Platform.BuildingBlocks.Abstractions;
using Microsoft.AspNetCore.Http;
using Platform.BuildingBlocks.Responses;
using Platform.Catalog.API.Application.Features.ProductCoverImages.Shared;
using Platform.Catalog.API.Application.Features.Products.Mappers;
using Platform.Catalog.API.Application.Features.Products.Shared;
using Platform.Catalog.API.Application.Features.Stores.Services;
using Platform.Catalog.API.Domain.Enums;
using Platform.Catalog.API.Infrastructure.Persistence.Models;

namespace Platform.Catalog.API.Application.Features.Products.Commands.Delete;

public sealed class DeleteProductHandler : ICommandHandler<DeleteProductCommand, ProductResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IBlobService _blobService;
    private readonly IStorePolicyService _storePolicyService;

    public DeleteProductHandler(IUnitOfWork unitOfWork, ICurrentUserProvider currentUserProvider, IBlobService blobService, IStorePolicyService storePolicyService)
    {
        _unitOfWork = unitOfWork;
        _currentUserProvider = currentUserProvider;
        _blobService = blobService;
        _storePolicyService = storePolicyService;
    }

    public async Task<Result<ProductResponse>> Handle(DeleteProductCommand command, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(_currentUserProvider.CurrentUserId, out var currentUserId))
            return Result<ProductResponse>.Failure(StatusCodes.Status401Unauthorized, "Current user is invalid.");

        var productModel = await _unitOfWork
            .GetRepository<ProductModel>()
            .FindAsync(
                x => x.Id == command.ProductId && x.Status != ProductStatus.Deleted,
                false,
                cancellationToken,
                x => x.Category,
                x => x.MediaFiles,
                x => x.CoverImage!);

        if (productModel is null)
            return Result<ProductResponse>.Failure(StatusCodes.Status404NotFound, "Product not found.");

        var manageDecision = await _storePolicyService.ResolveManageProductAsync(currentUserId, productModel.StoreId, productModel.CreatedBy, cancellationToken);
        if (manageDecision.Action == ManageStoreProductPolicyAction.StoreUnavailable)
            return Result<ProductResponse>.Failure(StatusCodes.Status403Forbidden, "Current user does not belong to the product store.");
        if (manageDecision.Action == ManageStoreProductPolicyAction.CreatorInvalid)
            return Result<ProductResponse>.Failure(StatusCodes.Status400BadRequest, "Product creator is invalid.");
        if (manageDecision.Action == ManageStoreProductPolicyAction.Forbidden)
            return Result<ProductResponse>.Failure(StatusCodes.Status403Forbidden, "Current user cannot delete this product.");

        var product = productModel.ToDomain();
        var deleteResult = product.Delete();

        if (deleteResult.IsFailure)
            return Result<ProductResponse>.Failure(StatusCodes.Status400BadRequest, "Unable to delete product.");

        productModel.ApplyDomainState(product);
        _unitOfWork.GetRepository<ProductModel>().Update(productModel);

        return Result<ProductResponse>.Success(product.ToResponse(productModel.ResolveCoverImageUrl(_blobService)));
    }
}
