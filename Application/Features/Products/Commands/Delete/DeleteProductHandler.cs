using Microsoft.EntityFrameworkCore;
using Platform.Application.Abstractions.Data;
using Platform.Application.Messaging;
using Platform.BuildingBlocks.Abstractions;
using Platform.BuildingBlocks.Responses;
using Platform.Catalog.API.Application.Features.Products.Shared;
using Platform.Catalog.API.Application.Mappers;
using Platform.Catalog.API.Domain.Enums;
using Platform.Catalog.API.Infrastructure.Persistence.Models;

namespace Platform.Catalog.API.Application.Features.Products.Commands.Delete;

public sealed class DeleteProductHandler : ICommandHandler<DeleteProductCommand, ProductResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserProvider _currentUserProvider;

    public DeleteProductHandler(IUnitOfWork unitOfWork, ICurrentUserProvider currentUserProvider)
    {
        _unitOfWork = unitOfWork;
        _currentUserProvider = currentUserProvider;
    }

    public async Task<Result<ProductResponse>> Handle(DeleteProductCommand command, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(_currentUserProvider.CurrentUserId, out var currentUserId))
            return Result<ProductResponse>.Failure("Current user is invalid.");

        var productModel = await _unitOfWork
            .GetRepository<ProductModel>()
            .GetQueryable()
            .Include(x => x.ProductTypes)
            .Include(x => x.MediaFiles)
            .FirstOrDefaultAsync(
                x => x.Id == command.ProductId && x.Status != ProductStatus.Deleted,
                cancellationToken);

        if (productModel is null)
            return Result<ProductResponse>.Failure("Product not found.");

        if (!Guid.TryParse(productModel.CreatedBy, out var ownerId) || ownerId != currentUserId)
            return Result<ProductResponse>.Failure("You do not own this product.");

        var product = productModel.ToDomain();
        var deleteResult = product.Delete();

        if (deleteResult.IsFailure)
            return Result<ProductResponse>.Failure("Unable to delete product.");

        productModel.ApplyDomainState(product, []);
        _unitOfWork.GetRepository<ProductModel>().Update(productModel);

        return Result<ProductResponse>.Success(productModel.ToResponse());
    }
}
