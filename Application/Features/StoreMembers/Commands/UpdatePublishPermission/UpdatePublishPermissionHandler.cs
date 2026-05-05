using MediatR;
using Platform.Application.Abstractions.Data;
using Platform.Application.Messaging;
using Platform.BuildingBlocks.Abstractions;
using Platform.BuildingBlocks.Responses;
using Platform.Catalog.API.Application.Features.StoreMembers.Mappers;
using Platform.Catalog.API.Domain.Enums;
using Platform.Catalog.API.Infrastructure.Persistence.Models;

namespace Platform.Catalog.API.Application.Features.StoreMembers.Commands.UpdatePublishPermission;

public sealed class UpdatePublishPermissionHandler : ICommandHandler<UpdatePublishPermissionCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserProvider _currentUserProvider;

    public UpdatePublishPermissionHandler(IUnitOfWork unitOfWork, ICurrentUserProvider currentUserProvider)
    {
        _unitOfWork = unitOfWork;
        _currentUserProvider = currentUserProvider;
    }

    public async Task<Result<Unit>> Handle(UpdatePublishPermissionCommand command, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(_currentUserProvider.CurrentUserId, out var currentUserId))
            return Result<Unit>.Failure(StatusCodes.Status401Unauthorized, "Current user is invalid.");

        var repository = _unitOfWork.GetRepository<StoreMemberModel>();
        var ownerMember = await repository.FindAsync(
            x => x.UserId == currentUserId
                && x.Role == StoreMemberRole.Owner
                && x.Status == StoreMemberStatus.Active
                && x.Store.Status == StoreStatus.Active
                && x.Store.IsVerified,
            true,
            cancellationToken,
            x => x.Store);

        if (ownerMember is null)
            return Result<Unit>.Failure(StatusCodes.Status403Forbidden, "Only the owner of a verified store can update publish permission.");

        var memberModel = await repository.FindAsync(
            x => x.StoreId == ownerMember.StoreId
                && x.UserId == command.UserId
                && x.Status == StoreMemberStatus.Active,
            false,
            cancellationToken);

        if (memberModel is null)
            return Result<Unit>.Failure(StatusCodes.Status404NotFound, "Store member not found.");

        var member = memberModel.ToDomain();
        var updateResult = member.UpdatePublishPermission(command.Request.CanPublishProductDirectly);
        if (updateResult.IsFailure)
            return Result<Unit>.Failure(StatusCodes.Status400BadRequest, "Unable to update publish permission.");

        memberModel.ApplyDomainState(member);
        repository.Update(memberModel);

        return Result<Unit>.Success(Unit.Value);
    }
}
