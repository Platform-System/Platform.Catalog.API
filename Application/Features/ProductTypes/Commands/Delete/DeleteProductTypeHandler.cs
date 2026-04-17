using MediatR;
using Microsoft.EntityFrameworkCore;
using Platform.Application.Abstractions.Data;
using Platform.Application.Messaging;
using Platform.BuildingBlocks.Responses;
using Platform.Catalog.API.Application.Features.ProductTypes.Shared;
using Platform.Catalog.API.Application.Mappers;
using Platform.Catalog.API.Infrastructure.Persistence.Models;

namespace Platform.Catalog.API.Application.Features.ProductTypes.Commands.Delete;

public sealed class DeleteProductTypeHandler : ICommandHandler<DeleteProductTypeCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteProductTypeHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Unit>> Handle(DeleteProductTypeCommand command, CancellationToken cancellationToken)
    {
        var productTypeModel = await _unitOfWork
            .GetRepository<ProductTypeModel>()
            .GetQueryable()
            .Include(x => x.Products)
            .FirstOrDefaultAsync(x => x.Id == command.ProductTypeId, cancellationToken);

        if (productTypeModel is null)
            return Result<Unit>.Failure("Product type not found.");

        var productType = productTypeModel.ToDomain();
        var deleteResult = productType.Delete();
        if (deleteResult.IsFailure)
            return Result<Unit>.Failure("Unable to delete product type.");

        productTypeModel.ApplyDomainState(productType);
        _unitOfWork.GetRepository<ProductTypeModel>().Update(productTypeModel);

        return Result<Unit>.Success(Unit.Value);
    }
}
