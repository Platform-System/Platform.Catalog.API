using FluentValidation;

namespace Platform.Catalog.API.Application.Features.Products.Commands.SetCover;

public sealed class SetProductCoverValidator : AbstractValidator<SetProductCoverCommand>
{
    public SetProductCoverValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty();

        RuleFor(x => x.Request.BlobName)
            .NotEmpty()
            .MaximumLength(255);

        RuleFor(x => x.Request.ContainerName)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.Request.FileName)
            .NotEmpty()
            .MaximumLength(255);

        RuleFor(x => x.Request.ContentType)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Request.Size)
            .GreaterThan(0);

        RuleFor(x => x.Request.AltText)
            .NotEmpty()
            .MaximumLength(500);
    }
}
