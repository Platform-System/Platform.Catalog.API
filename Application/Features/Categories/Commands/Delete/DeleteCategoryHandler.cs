using MediatR;
using Microsoft.AspNetCore.Http;
using Platform.Application.Abstractions.Data;
using Platform.Application.Messaging;
using Platform.BuildingBlocks.Responses;
using Platform.Catalog.API.Application.Features.Categories.Mappers;
using Platform.Catalog.API.Application.Features.Categories.Shared;
using Platform.Catalog.API.Infrastructure.Persistence.Models;

namespace Platform.Catalog.API.Application.Features.Categories.Commands.Delete;

public sealed class DeleteCategoryHandler : ICommandHandler<DeleteCategoryCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCategoryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Unit>> Handle(DeleteCategoryCommand command, CancellationToken cancellationToken)
    {
        var categoryModel = await _unitOfWork
            .GetRepository<CategoryModel>()
            .FindAsync(
                x => x.Id == command.CategoryId,
                false,
                cancellationToken,
                x => x.Products);

        if (categoryModel is null)
            return Result<Unit>.Failure(StatusCodes.Status404NotFound, "Category not found.");

        var category = categoryModel.ToDomain();
        var deleteResult = category.Delete();
        if (deleteResult.IsFailure)
            return Result<Unit>.Failure(StatusCodes.Status400BadRequest, "Unable to delete category.");

        categoryModel.ApplyDomainState(category);
        _unitOfWork.GetRepository<CategoryModel>().Update(categoryModel);

        return Result<Unit>.Success(Unit.Value);
    }
}
