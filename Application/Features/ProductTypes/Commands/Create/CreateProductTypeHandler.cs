using Platform.Application.Abstractions.Data;
using Platform.Application.Messaging;
using Platform.BuildingBlocks.Responses;
using Platform.Catalog.API.Application.Features.ProductTypes.Shared;
using Platform.Catalog.API.Domain.Entities;
using Platform.Catalog.API.Infrastructure.Persistence.Models;

namespace Platform.Catalog.API.Application.Features.ProductTypes.Commands.Create;

public sealed class CreateProductTypeHandler : ICommandHandler<CreateProductTypeCommand, ProductTypeResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateProductTypeHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ProductTypeResponse>> Handle(CreateProductTypeCommand command, CancellationToken cancellationToken)
    {
        var createResult = ProductType.Create(command.Request.Name);
        if (createResult.IsFailure)
            return Result<ProductTypeResponse>.Failure("Unable to create product type.");

        var productType = createResult.Value;
        var productTypeModel = productType.ToPersistence();

        await _unitOfWork.GetRepository<ProductTypeModel>().AddAsync(productTypeModel, cancellationToken);

        return Result<ProductTypeResponse>.Success(productTypeModel.ToResponse());
    }
}
