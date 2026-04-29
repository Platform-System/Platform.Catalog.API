using Platform.Application.Abstractions.Data;
using Platform.Application.Messaging;
using Platform.BuildingBlocks.Responses;
using Platform.Catalog.API.Application.Features.Categories.Shared;
using Platform.Catalog.API.Domain.Entities;
using Platform.Catalog.API.Infrastructure.Persistence.Models;

namespace Platform.Catalog.API.Application.Features.Categories.Commands.Create;

public sealed class CreateCategoryHandler : ICommandHandler<CreateCategoryCommand, CategoryResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateCategoryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CategoryResponse>> Handle(CreateCategoryCommand command, CancellationToken cancellationToken)
    {
        var createResult = Category.Create(command.Request.Name);
        if (createResult.IsFailure)
            return Result<CategoryResponse>.Failure("Unable to create category.");

        var category = createResult.Value;
        var categoryModel = category.ToPersistence();

        await _unitOfWork.GetRepository<CategoryModel>().AddAsync(categoryModel, cancellationToken);

        return Result<CategoryResponse>.Success(categoryModel.ToResponse());
    }
}
