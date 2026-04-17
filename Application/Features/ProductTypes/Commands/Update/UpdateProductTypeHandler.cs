using Microsoft.EntityFrameworkCore;
using Platform.Application.Abstractions.Data;
using Platform.Application.Messaging;
using Platform.BuildingBlocks.Responses;
using Platform.Catalog.API.Application.Features.ProductTypes.Shared;
using Platform.Catalog.API.Application.Mappers;
using Platform.Catalog.API.Domain.Enums;
using Platform.Catalog.API.Infrastructure.Persistence.Models;

namespace Platform.Catalog.API.Application.Features.ProductTypes.Commands.Update;

public sealed class UpdateProductTypeHandler : ICommandHandler<UpdateProductTypeCommand, ProductTypeResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProductTypeHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ProductTypeResponse>> Handle(UpdateProductTypeCommand command, CancellationToken cancellationToken)
    {
        var productTypeModel = await _unitOfWork
            .GetRepository<ProductTypeModel>()
            .GetQueryable()
            .FirstOrDefaultAsync(
                x => x.Id == command.ProductTypeId && x.Status == ProductTypeStatus.Active,
                cancellationToken);

        if (productTypeModel is null)
            return Result<ProductTypeResponse>.Failure("Product type not found.");

        var productType = productTypeModel.ToDomain();
        var updateResult = productType.UpdateName(command.Request.Name);
        if (updateResult.IsFailure)
            return Result<ProductTypeResponse>.Failure("Unable to update product type.");

        productTypeModel.ApplyDomainState(productType);
        _unitOfWork.GetRepository<ProductTypeModel>().Update(productTypeModel);

        return Result<ProductTypeResponse>.Success(productTypeModel.ToResponse());
    }
}
