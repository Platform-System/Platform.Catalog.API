using Platform.Application.Abstractions.Data;
using Platform.Application.Abstractions.Storage;
using Platform.Application.Messaging;
using Platform.BuildingBlocks.Abstractions;
using Microsoft.AspNetCore.Http;
using Platform.BuildingBlocks.Responses;
using Platform.Catalog.API.Application.Abstractions.Stores;
using Platform.Catalog.API.Application.Features.Categories.Mappers;
using Platform.Catalog.API.Application.Features.ProductCoverImages.Responses;
using Platform.Catalog.API.Application.Features.Products.Mappers;
using Platform.Catalog.API.Application.Features.Products.Responses;
using Platform.Catalog.API.Domain.Enums;
using Platform.Catalog.API.Infrastructure.Persistence.Models;

namespace Platform.Catalog.API.Application.Features.Products.Commands.Update;

public sealed class UpdateProductHandler : ICommandHandler<UpdateProductCommand, ProductResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IBlobService _blobService;
    private readonly IStorePolicyService _storePolicyService;

    public UpdateProductHandler(IUnitOfWork unitOfWork, ICurrentUserProvider currentUserProvider, IBlobService blobService, IStorePolicyService storePolicyService)
    {
        _unitOfWork = unitOfWork;
        _currentUserProvider = currentUserProvider;
        _blobService = blobService;
        _storePolicyService = storePolicyService;
    }

    public async Task<Result<ProductResponse>> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(_currentUserProvider.CurrentUserId, out var currentUserId))
            return Result<ProductResponse>.Failure(StatusCodes.Status401Unauthorized, "Current user is invalid.");

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
            return Result<ProductResponse>.Failure(StatusCodes.Status404NotFound, "Product not found.");

        if (productModel.Status == ProductStatus.Active)
            return Result<ProductResponse>.Failure(StatusCodes.Status400BadRequest, "Active product cannot be updated.");

        var categoryModel = await _unitOfWork
            .GetRepository<CategoryModel>()
            .FindAsync(x => x.Id == command.Request.CategoryId && x.Status == CategoryStatus.Active, true, cancellationToken);

        if (categoryModel is null)
            return Result<ProductResponse>.Failure(StatusCodes.Status400BadRequest, "Category is invalid.");

        var manageDecision = await _storePolicyService.ResolveManageProductAsync(currentUserId, productModel.StoreId, productModel.CreatedBy, cancellationToken);
        if (manageDecision.Action == ManageStoreProductPolicyAction.StoreUnavailable)
            return Result<ProductResponse>.Failure(StatusCodes.Status403Forbidden, "Current user does not belong to the product store.");
        if (manageDecision.Action == ManageStoreProductPolicyAction.CreatorInvalid)
            return Result<ProductResponse>.Failure(StatusCodes.Status400BadRequest, "Product creator is invalid.");
        if (manageDecision.Action == ManageStoreProductPolicyAction.Forbidden)
            return Result<ProductResponse>.Failure(StatusCodes.Status403Forbidden, "Current user cannot update this product.");

        var product = productModel.ToDomain();
        var category = categoryModel.ToDomain();

        var stockResult = product.UpdateStock(command.Request.Stock);
        if (stockResult.IsFailure)
        {
            return Result<ProductResponse>.Failure(StatusCodes.Status400BadRequest, "Unable to update product stock.");
        }

        var updateInfoResult = product.UpdateInfo(
            command.Request.Title,
            command.Request.Author,
            command.Request.Price,
            category);

        if (updateInfoResult.IsFailure)
            return Result<ProductResponse>.Failure(StatusCodes.Status400BadRequest, "Unable to update product information.");

        productModel.ApplyDomainState(product, categoryModel);
        _unitOfWork.GetRepository<ProductModel>().Update(productModel);
        return Result<ProductResponse>.Success(product.ToResponse(productModel.ResolveCoverImageUrl(_blobService)));
    }
}
