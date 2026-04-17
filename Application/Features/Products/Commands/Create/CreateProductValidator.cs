using FluentValidation;
using Platform.Catalog.API.Application.Features.Products.Shared;

namespace Platform.Catalog.API.Application.Features.Products.Commands.Create;

public sealed class CreateProductValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.BlobName)
            .NotEmpty()
            .WithMessage("Blob name is required.");

        RuleFor(x => x.ContainerName)
            .NotEmpty()
            .WithMessage("Container name is required.");

        RuleFor(x => x.Request.Title)
            .NotEmpty()
            .WithMessage("Product title is required.");

        RuleFor(x => x.Request.Author)
            .NotEmpty()
            .WithMessage("Author name cannot be empty.");

        RuleFor(x => x.Request.Price)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Price must be zero or a positive value.");

        RuleFor(x => x.Request.ProductTypeIds)
            .NotEmpty()
            .WithMessage("At least one product type is required.");

        RuleFor(x => x.Request.ProductTypeIds)
            .Must(ids => ids is not null && ids.Distinct().Count() == ids.Count)
            .WithMessage("Product types must not contain duplicates.");

        When(x => x.Request.Kind == ProductKind.PhysicalProduct, () =>
        {
            RuleFor(x => x.Request.Stock)
                .NotNull()
                .WithMessage("Stock count is required for physical products.");

            RuleFor(x => x.Request.Stock)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Stock cannot be a negative number.");
        });
    }
}
