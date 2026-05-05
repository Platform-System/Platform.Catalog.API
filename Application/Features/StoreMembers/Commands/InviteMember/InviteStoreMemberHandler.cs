using MediatR;
using Platform.Application.Abstractions.Data;
using Platform.Application.Messaging;
using Platform.BuildingBlocks.Abstractions;
using Platform.BuildingBlocks.Responses;
using Platform.Catalog.API.Application.Features.StoreMembers.Mappers;
using Platform.Catalog.API.Domain.Entities;
using Platform.Catalog.API.Domain.Enums;
using Platform.Catalog.API.Infrastructure.Persistence.Models;

namespace Platform.Catalog.API.Application.Features.StoreMembers.Commands.InviteMember;

public sealed class InviteStoreMemberHandler : ICommandHandler<InviteStoreMemberCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserProvider _currentUserProvider;

    public InviteStoreMemberHandler(IUnitOfWork unitOfWork, ICurrentUserProvider currentUserProvider)
    {
        _unitOfWork = unitOfWork;
        _currentUserProvider = currentUserProvider;
    }

    public async Task<Result<Unit>> Handle(InviteStoreMemberCommand command, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(_currentUserProvider.CurrentUserId, out var currentUserId))
            return Result<Unit>.Failure(StatusCodes.Status401Unauthorized, "Current user is invalid.");

        var storeMemberRepository = _unitOfWork.GetRepository<StoreMemberModel>();

        var ownerMember = await storeMemberRepository.FindAsync(
            x => x.StoreId == command.StoreId
                && x.UserId == currentUserId
                && x.Role == StoreMemberRole.Owner
                && x.Status == StoreMemberStatus.Active
                && x.Store.Status == StoreStatus.Active
                && x.Store.IsVerified,
            true,
            cancellationToken,
            x => x.Store);

        if (ownerMember is null)
            return Result<Unit>.Failure(StatusCodes.Status403Forbidden, "Only the owner of a verified store can invite members.");

        if (command.Request.UserId == currentUserId)
            return Result<Unit>.Failure(StatusCodes.Status400BadRequest, "Owner does not need an invitation.");

        var existingMember = await storeMemberRepository.FindAsync(x => x.UserId == command.Request.UserId, true, cancellationToken);
        if (existingMember is not null)
            return Result<Unit>.Failure(StatusCodes.Status400BadRequest, "User already belongs to a store or has a pending invitation.");

        var invite = StoreMember.Invite(command.StoreId, command.Request.UserId, command.Request.Role, command.Request.CanPublishProductDirectly);
        await storeMemberRepository.AddAsync(invite.ToPersistence(), cancellationToken);

        return Result<Unit>.Success(Unit.Value);
    }
}
