using FluentValidation;

namespace Platform.Catalog.API.Application.Features.Stores.Commands.Update;

public sealed class UpdateStoreValidator : AbstractValidator<UpdateStoreCommand>
{
    public UpdateStoreValidator()
    {
        RuleFor(x => x.Request.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Request.Description)
            .MaximumLength(4000)
            .When(x => !string.IsNullOrWhiteSpace(x.Request.Description));

        RuleFor(x => x.Request.Tagline)
            .MaximumLength(250)
            .When(x => !string.IsNullOrWhiteSpace(x.Request.Tagline));

        RuleFor(x => x.Request.Location)
            .MaximumLength(250)
            .When(x => !string.IsNullOrWhiteSpace(x.Request.Location));

        RuleFor(x => x.Request.ResponseTime)
            .MaximumLength(150)
            .When(x => !string.IsNullOrWhiteSpace(x.Request.ResponseTime));

        RuleFor(x => x.Request.AvatarUrl)
            .MaximumLength(1000)
            .When(x => !string.IsNullOrWhiteSpace(x.Request.AvatarUrl));

        RuleFor(x => x.Request.CoverImageUrl)
            .MaximumLength(1000)
            .When(x => !string.IsNullOrWhiteSpace(x.Request.CoverImageUrl));

        RuleFor(x => x.Request.ShippingPolicy)
            .MaximumLength(2000)
            .When(x => !string.IsNullOrWhiteSpace(x.Request.ShippingPolicy));

        RuleFor(x => x.Request.ReturnPolicy)
            .MaximumLength(2000)
            .When(x => !string.IsNullOrWhiteSpace(x.Request.ReturnPolicy));

        RuleFor(x => x.Request.WarrantyPolicy)
            .MaximumLength(2000)
            .When(x => !string.IsNullOrWhiteSpace(x.Request.WarrantyPolicy));
    }
}
