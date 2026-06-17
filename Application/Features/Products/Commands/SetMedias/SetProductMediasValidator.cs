using FluentValidation;

namespace Platform.Catalog.API.Application.Features.Products.Commands.SetMedias;

public sealed class SetProductMediasValidator : AbstractValidator<SetProductMediasCommand>
{
    public SetProductMediasValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty();

        RuleFor(x => x.Request.Items)
            .NotEmpty()
            .WithMessage("At least one media item is required.");

        RuleForEach(x => x.Request.Items)
            .ChildRules(item =>
            {
                item.RuleFor(x => x.BlobName)
                    .NotEmpty()
                    .MaximumLength(255);

                item.RuleFor(x => x.ContainerName)
                    .NotEmpty()
                    .MaximumLength(150);

                item.RuleFor(x => x.FileName)
                    .NotEmpty()
                    .MaximumLength(255);

                item.RuleFor(x => x.ContentType)
                    .NotEmpty()
                    .MaximumLength(100);

                item.RuleFor(x => x.Size)
                    .GreaterThan(0);

                item.RuleFor(x => x.AltText)
                    .NotEmpty()
                    .MaximumLength(500);
            });
    }
}
