using Microsoft.EntityFrameworkCore;
using Platform.Application.Abstractions.Data;
using Platform.Application.Messaging;
using Platform.BuildingBlocks.Responses;
using Platform.Catalog.API.Application.Features.Categories.Shared;
using Platform.Catalog.API.Application.Mappers;
using Platform.Catalog.API.Domain.Enums;
using Platform.Catalog.API.Infrastructure.Persistence.Models;

namespace Platform.Catalog.API.Application.Features.Categories.Commands.Update;

public sealed class UpdateCategoryHandler : ICommandHandler<UpdateCategoryCommand, CategoryResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCategoryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CategoryResponse>> Handle(UpdateCategoryCommand command, CancellationToken cancellationToken)
    {
        var categoryModel = await _unitOfWork
            .GetRepository<CategoryModel>()
            .GetQueryable()
            .FirstOrDefaultAsync(
                x => x.Id == command.CategoryId && x.Status == CategoryStatus.Active,
                cancellationToken);

        if (categoryModel is null)
            return Result<CategoryResponse>.Failure("Category not found.");

        var category = categoryModel.ToDomain();
        var updateResult = category.UpdateName(command.Request.Name);
        if (updateResult.IsFailure)
            return Result<CategoryResponse>.Failure("Unable to update category.");

        categoryModel.ApplyDomainState(category);
        _unitOfWork.GetRepository<CategoryModel>().Update(categoryModel);

        return Result<CategoryResponse>.Success(categoryModel.ToResponse());
    }
}
