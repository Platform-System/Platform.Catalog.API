using FluentValidation;

namespace Platform.Catalog.API.Application.Features.ProductTypes.Commands.Create;

public sealed class CreateProductTypeValidator : AbstractValidator<CreateProductTypeCommand>
{
    public CreateProductTypeValidator()
    {
        RuleFor(x => x.Request.Name)
            .NotEmpty()
            .WithMessage("Name is required.");
    }
}
