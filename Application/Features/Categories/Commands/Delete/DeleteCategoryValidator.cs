using FluentValidation;

namespace Platform.Catalog.API.Application.Features.Categories.Commands.Delete;

public sealed class DeleteCategoryValidator : AbstractValidator<DeleteCategoryCommand>
{
    public DeleteCategoryValidator()
    {
        RuleFor(x => x.CategoryId)
            .NotEmpty()
            .WithMessage("Category id is required.");
    }
}
