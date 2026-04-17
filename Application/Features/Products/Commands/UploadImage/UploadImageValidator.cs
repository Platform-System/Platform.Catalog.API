using FluentValidation;

namespace Platform.Catalog.API.Application.Features.Products.Commands.UploadImage;

public sealed class UploadImageValidator : AbstractValidator<UploadImageCommand>
{
    public UploadImageValidator()
    {
        RuleFor(x => x.Request.Stream)
            .NotNull()
            .WithMessage("Image stream is required.");

        RuleFor(x => x.Request.FileName)
            .NotEmpty()
            .WithMessage("File name is required.");

        RuleFor(x => x.Request.ContentType)
            .NotEmpty()
            .WithMessage("Content type is required.");
    }
}
