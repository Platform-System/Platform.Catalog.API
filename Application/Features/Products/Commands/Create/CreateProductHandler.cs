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
using Platform.Catalog.API.Domain.Entities;
using Platform.Catalog.API.Domain.Enums;
using Platform.Catalog.API.Infrastructure.Persistence.Models;

namespace Platform.Catalog.API.Application.Features.Products.Commands.Create;

public sealed class CreateProductHandler : ICommandHandler<CreateProductCommand, ProductResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IBlobService _blobService;
    private readonly IStorePolicyService _storePolicyService;

    public CreateProductHandler(IUnitOfWork unitOfWork, ICurrentUserProvider currentUserProvider, IBlobService blobService, IStorePolicyService storePolicyService)
    {
        _unitOfWork = unitOfWork;
        _currentUserProvider = currentUserProvider;
        _blobService = blobService;
        _storePolicyService = storePolicyService;
    }

    public async Task<Result<ProductResponse>> Handle(CreateProductCommand command, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(_currentUserProvider.CurrentUserId, out var currentUserId))
            return Result<ProductResponse>.Failure(StatusCodes.Status401Unauthorized, "Current user is invalid.");

        var categoryModel = await _unitOfWork
            .GetRepository<CategoryModel>()
            .FindAsync(x => x.Id == command.Request.CategoryId && x.Status == CategoryStatus.Active, true, cancellationToken);

        if (categoryModel is null)
            return Result<ProductResponse>.Failure(StatusCodes.Status400BadRequest, "Category is invalid.");

        var storeDecision = await _storePolicyService.ResolveCreateProductAsync(currentUserId, cancellationToken);
        if (storeDecision.Action == CreateProductStorePolicyAction.StoreUnavailable)
            return Result<ProductResponse>.Failure(StatusCodes.Status400BadRequest, "Current user does not belong to an available store.");
        if (storeDecision.Action == CreateProductStorePolicyAction.OwnerRequiredForUnverifiedStore)
            return Result<ProductResponse>.Failure(StatusCodes.Status403Forbidden, "Only the store owner can create products before the store is verified.");

        var createResult = Product.Create(
            command.Request.Title,
            command.Request.Author,
            command.Request.Price,
            storeDecision.StoreId,
            categoryModel.ToDomain(),
            command.Request.Stock);

        if (!createResult.IsSuccess)
            return Result<ProductResponse>.Failure(StatusCodes.Status400BadRequest, "Unable to create product.");

        var product = createResult.Value;
        product.SetDraft();
        var productModel = product.ToPersistence(categoryModel);

        await _unitOfWork.GetRepository<ProductModel>().AddAsync(productModel, cancellationToken);
        return Result<ProductResponse>.Success(product.ToResponse(productModel.ResolveCoverImageUrl(_blobService)));
    }
}
