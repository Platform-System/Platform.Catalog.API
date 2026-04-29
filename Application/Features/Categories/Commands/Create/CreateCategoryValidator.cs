using FluentValidation;

namespace Platform.Catalog.API.Application.Features.Categories.Commands.Create;

public sealed class CreateCategoryValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryValidator()
    {
        RuleFor(x => x.Request.Name)
            .NotEmpty()
            .WithMessage("Name is required.");
    }
}
