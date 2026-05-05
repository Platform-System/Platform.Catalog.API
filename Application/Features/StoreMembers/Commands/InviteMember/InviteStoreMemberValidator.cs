using FluentValidation;
using Platform.Catalog.API.Domain.Enums;

namespace Platform.Catalog.API.Application.Features.StoreMembers.Commands.InviteMember;

public sealed class InviteStoreMemberValidator : AbstractValidator<InviteStoreMemberCommand>
{
    public InviteStoreMemberValidator()
    {
        RuleFor(x => x.Request.UserId)
            .NotEmpty();

        RuleFor(x => x.Request.Role)
            .Must(role => role == StoreMemberRole.Manager || role == StoreMemberRole.Staff)
            .WithMessage("Only Manager or Staff can be invited.");
    }
}
