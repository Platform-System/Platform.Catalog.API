using FluentValidation;

namespace Platform.Catalog.API.Application.Features.Products.Commands.SetCover;

public sealed class SetProductCoverValidator : AbstractValidator<SetProductCoverCommand>
{
    public SetProductCoverValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("Product id is required.");

        RuleFor(x => x.Request.BlobName)
            .NotEmpty()
            .MaximumLength(255)
            .WithMessage("Blob name is required.");

        RuleFor(x => x.Request.ContainerName)
            .NotEmpty()
            .MaximumLength(150)
            .WithMessage("Container name is required.");

        RuleFor(x => x.Request.FileName)
            .NotEmpty()
            .MaximumLength(255)
            .WithMessage("File name is required.");

        RuleFor(x => x.Request.ContentType)
            .NotEmpty()
            .MaximumLength(100)
            .WithMessage("Content type is required.");

        RuleFor(x => x.Request.Size)
            .GreaterThan(0)
            .WithMessage("File size must be greater than 0.");
    }
}
