using FluentValidation;

namespace Platform.Catalog.API.Application.Features.ProductTypes.Commands.Delete;

public sealed class DeleteProductTypeValidator : AbstractValidator<DeleteProductTypeCommand>
{
    public DeleteProductTypeValidator()
    {
        RuleFor(x => x.ProductTypeId)
            .NotEmpty()
            .WithMessage("Product type id is required.");
    }
}
