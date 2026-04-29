using FluentValidation;

namespace Platform.Catalog.API.Application.Features.Categories.Commands.Update;

public sealed class UpdateCategoryValidator : AbstractValidator<UpdateCategoryCommand>
{
    public UpdateCategoryValidator()
    {
        RuleFor(x => x.CategoryId)
            .NotEmpty()
            .WithMessage("Category id is required.");

        RuleFor(x => x.Request.Name)
            .NotEmpty()
            .WithMessage("Name is required.");
    }
}
