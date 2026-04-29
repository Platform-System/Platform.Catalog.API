using Microsoft.EntityFrameworkCore;
using Platform.Application.Abstractions.Data;
using Platform.Application.Messaging;
using Platform.BuildingBlocks.Abstractions;
using Platform.BuildingBlocks.Responses;
using Platform.Catalog.API.Application.Features.Products.Shared;
using Platform.Catalog.API.Application.Mappers;
using Platform.Catalog.API.Domain.Enums;
using Platform.Catalog.API.Infrastructure.Persistence.Models;

namespace Platform.Catalog.API.Application.Features.Products.Commands.Update;

public sealed class UpdateProductHandler : ICommandHandler<UpdateProductCommand, ProductResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserProvider _currentUserProvider;

    public UpdateProductHandler(IUnitOfWork unitOfWork, ICurrentUserProvider currentUserProvider)
    {
        _unitOfWork = unitOfWork;
        _currentUserProvider = currentUserProvider;
    }

    public async Task<Result<ProductResponse>> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(_currentUserProvider.CurrentUserId, out var currentUserId))
            return Result<ProductResponse>.Failure("Current user is invalid.");

        var productModel = await _unitOfWork
            .GetRepository<ProductModel>()
            .GetQueryable()
            .Include(x => x.Category)
            .Include(x => x.MediaFiles)
            .FirstOrDefaultAsync(x => x.Id == command.ProductId, cancellationToken);

        if (productModel is null || productModel.Status == ProductStatus.Deleted)
            return Result<ProductResponse>.Failure("Product not found.");

        if(productModel.Status == ProductStatus.Active)
            return Result<ProductResponse>.Failure("Active product cannot be updated.");

        if (!Guid.TryParse(productModel.CreatedBy, out var ownerId) || ownerId != currentUserId)
            return Result<ProductResponse>.Failure("You do not own this product.");

        var categoryModel = await _unitOfWork
            .GetRepository<CategoryModel>()
            .FindAsync(x => x.Id == command.Request.CategoryId && x.Status == CategoryStatus.Active, true, cancellationToken);

        if (categoryModel is null)
            return Result<ProductResponse>.Failure("Category is invalid.");

        var product = productModel.ToDomain();
        var category = categoryModel.ToDomain();

        var stockResult = product.UpdateStock(command.Request.Stock);
        if (stockResult.IsFailure)
        {
            return Result<ProductResponse>.Failure("Unable to update product stock.");
        }

        var updateInfoResult = product.UpdateInfo(
            command.Request.Title,
            command.Request.Author,
            command.Request.Price,
            category);

        if (updateInfoResult.IsFailure)
            return Result<ProductResponse>.Failure("Unable to update product information.");

        productModel.ApplyDomainState(product, categoryModel);
        _unitOfWork.GetRepository<ProductModel>().Update(productModel);
        return Result<ProductResponse>.Success(productModel.ToResponse());
    }
}
