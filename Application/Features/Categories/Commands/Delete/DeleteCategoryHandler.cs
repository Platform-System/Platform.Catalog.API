using MediatR;
using Microsoft.EntityFrameworkCore;
using Platform.Application.Abstractions.Data;
using Platform.Application.Messaging;
using Platform.BuildingBlocks.Responses;
using Platform.Catalog.API.Application.Features.Categories.Shared;
using Platform.Catalog.API.Application.Mappers;
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
            .GetQueryable()
            .Include(x => x.Products)
            .FirstOrDefaultAsync(x => x.Id == command.CategoryId, cancellationToken);

        if (categoryModel is null)
            return Result<Unit>.Failure("Category not found.");

        var category = categoryModel.ToDomain();
        var deleteResult = category.Delete();
        if (deleteResult.IsFailure)
            return Result<Unit>.Failure("Unable to delete category.");

        categoryModel.ApplyDomainState(category);
        _unitOfWork.GetRepository<CategoryModel>().Update(categoryModel);

        return Result<Unit>.Success(Unit.Value);
    }
}
