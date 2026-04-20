using Microsoft.EntityFrameworkCore;
using Platform.Application.Abstractions.Data;
using Platform.Application.Messaging;
using Platform.BuildingBlocks.Abstractions;
using Platform.BuildingBlocks.Responses;
using Platform.Catalog.API.Application.Features.Products.Shared;
using Platform.Catalog.API.Application.Mappers;
using Platform.Catalog.API.Domain.Entities;
using Platform.Catalog.API.Domain.Enums;
using Platform.Catalog.API.Infrastructure.Persistence.Models;

using Platform.SharedKernel.Enums;

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
            .Include(x => x.ProductTypes)
            .Include(x => x.MediaFiles)
            .FirstOrDefaultAsync(x => x.Id == command.ProductId, cancellationToken);

        if (productModel is null || productModel.Status == ProductStatus.Deleted)
            return Result<ProductResponse>.Failure("Product not found.");

        if(productModel.Status == ProductStatus.Active)
            return Result<ProductResponse>.Failure("Active product cannot be updated.");

        if (!Guid.TryParse(productModel.CreatedBy, out var ownerId) || ownerId != currentUserId)
            return Result<ProductResponse>.Failure("You do not own this product.");

        var requestedTypeIds = command.Request.ProductTypeIds.Distinct().ToList();

        var productTypeModels = await _unitOfWork
            .GetRepository<ProductTypeModel>()
            .GetQueryable()
            .Where(x => requestedTypeIds.Contains(x.Id) && x.Status == ProductTypeStatus.Active)
            .ToListAsync(cancellationToken);

        if (productTypeModels.Count != requestedTypeIds.Count)
            return Result<ProductResponse>.Failure("One or more product types are invalid.");

        var product = productModel.ToDomain();
        var productTypes = productTypeModels.Select(x => x.ToDomain()).ToList();

        var currentKind = productModel is PhysicalProductModel
            ? ProductKind.PhysicalProduct
            : ProductKind.DigitalProduct;

        if (command.Request.Kind != currentKind)
            return Result<ProductResponse>.Failure("Product kind cannot be changed.");

        if (product is PhysicalProduct physicalProduct)
        {
            if (command.Request.Stock is null)
            {
                return Result<ProductResponse>.Failure("Stock count is required for physical products.");
            }

            var stockResult = physicalProduct.UpdateStock(command.Request.Stock.Value);
            if (stockResult.IsFailure)
            {
                return Result<ProductResponse>.Failure("Unable to update product stock.");
            }
        }
        else if (command.Request.Stock is not null)
        {
            return Result<ProductResponse>.Failure("Digital product cannot have stock.");
        }

        var updateInfoResult = product.UpdateInfo(
            command.Request.Title,
            command.Request.Author,
            command.Request.Price,
            productTypes);

        if (updateInfoResult.IsFailure)
            return Result<ProductResponse>.Failure("Unable to update product information.");

        productModel.ApplyDomainState(product, productTypeModels);
        _unitOfWork.GetRepository<ProductModel>().Update(productModel);
        return Result<ProductResponse>.Success(productModel.ToResponse());
    }
}
