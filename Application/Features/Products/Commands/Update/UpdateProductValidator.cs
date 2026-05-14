using FluentValidation;
using Platform.Catalog.API.Application.Features.Products.Responses;

namespace Platform.Catalog.API.Application.Features.Products.Commands.Update;

public sealed class UpdateProductValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("Product id is required.");

        RuleFor(x => x.Request.Title)
            .NotEmpty()
            .WithMessage("Product title is required.");

        RuleFor(x => x.Request.Author)
            .NotEmpty()
            .WithMessage("Author name cannot be empty.");

        RuleFor(x => x.Request.Price)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Price must be zero or a positive value.");

        RuleFor(x => x.Request.CategoryId)
            .NotEmpty()
            .WithMessage("Category is required.");

        RuleFor(x => x.Request.Stock)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Stock cannot be a negative number.");
    }
}
