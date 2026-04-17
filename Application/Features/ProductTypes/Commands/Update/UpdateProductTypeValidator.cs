using FluentValidation;

namespace Platform.Catalog.API.Application.Features.ProductTypes.Commands.Update;

public sealed class UpdateProductTypeValidator : AbstractValidator<UpdateProductTypeCommand>
{
    public UpdateProductTypeValidator()
    {
        RuleFor(x => x.ProductTypeId)
            .NotEmpty()
            .WithMessage("Product type id is required.");

        RuleFor(x => x.Request.Name)
            .NotEmpty()
            .WithMessage("Name is required.");
    }
}
