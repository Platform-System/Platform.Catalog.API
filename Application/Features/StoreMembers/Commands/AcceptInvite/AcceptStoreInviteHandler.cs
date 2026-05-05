using MediatR;
using Platform.Application.Abstractions.Data;
using Platform.Application.Messaging;
using Platform.BuildingBlocks.Abstractions;
using Platform.BuildingBlocks.Responses;
using Platform.Catalog.API.Application.Features.StoreMembers.Mappers;
using Platform.Catalog.API.Domain.Enums;
using Platform.Catalog.API.Infrastructure.Persistence.Models;

namespace Platform.Catalog.API.Application.Features.StoreMembers.Commands.AcceptInvite;

public sealed class AcceptStoreInviteHandler : ICommandHandler<AcceptStoreInviteCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserProvider _currentUserProvider;

    public AcceptStoreInviteHandler(IUnitOfWork unitOfWork, ICurrentUserProvider currentUserProvider)
    {
        _unitOfWork = unitOfWork;
        _currentUserProvider = currentUserProvider;
    }

    public async Task<Result<Unit>> Handle(AcceptStoreInviteCommand command, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(_currentUserProvider.CurrentUserId, out var currentUserId))
            return Result<Unit>.Failure(StatusCodes.Status401Unauthorized, "Current user is invalid.");

        var storeMemberRepository = _unitOfWork.GetRepository<StoreMemberModel>();
        var storeMemberModel = await storeMemberRepository.FindAsync(
            x => x.StoreId == command.StoreId
                && x.UserId == currentUserId
                && x.Status == StoreMemberStatus.Invited
                && x.Store.Status == StoreStatus.Active
                && x.Store.IsVerified,
            false,
            cancellationToken,
            x => x.Store);

        if (storeMemberModel is null)
            return Result<Unit>.Failure(StatusCodes.Status404NotFound, "Invitation not found.");

        var member = storeMemberModel.ToDomain();
        var acceptResult = member.AcceptInvite();
        if (acceptResult.IsFailure)
            return Result<Unit>.Failure(StatusCodes.Status400BadRequest, "Invitation cannot be accepted.");

        storeMemberModel.ApplyDomainState(member);
        storeMemberRepository.Update(storeMemberModel);

        return Result<Unit>.Success(Unit.Value);
    }
}
