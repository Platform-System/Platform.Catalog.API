using FluentValidation;

namespace Platform.Catalog.API.Application.Features.Products.Commands.Delete;

public sealed class DeleteProductValidator : AbstractValidator<DeleteProductCommand>
{
    public DeleteProductValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("Product id is required.");
    }
}
