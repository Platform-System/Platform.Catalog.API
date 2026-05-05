using MediatR;
using Platform.Application.Abstractions.Data;
using Platform.Application.Messaging;
using Platform.BuildingBlocks.Abstractions;
using Platform.BuildingBlocks.Responses;
using Platform.Catalog.API.Infrastructure.Persistence.Models;

namespace Platform.Catalog.API.Application.Features.Stores.Commands.RequestVerification;

public sealed class RequestStoreVerificationHandler : ICommandHandler<RequestStoreVerificationCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserProvider _currentUserProvider;

    public RequestStoreVerificationHandler(IUnitOfWork unitOfWork, ICurrentUserProvider currentUserProvider)
    {
        _unitOfWork = unitOfWork;
        _currentUserProvider = currentUserProvider;
    }

    public async Task<Result<Unit>> Handle(RequestStoreVerificationCommand command, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(_currentUserProvider.CurrentUserId, out var currentUserId))
            return Result<Unit>.Failure(StatusCodes.Status401Unauthorized, "Current user is invalid.");

        var storeMemberModel = await _unitOfWork
            .GetRepository<StoreMemberModel>()
            .FindAsync(
                x => x.StoreId == command.StoreId
                    && x.UserId == currentUserId
                    && x.Role == Domain.Enums.StoreMemberRole.Owner
                    && x.Status == Domain.Enums.StoreMemberStatus.Active,
                false,
                cancellationToken,
                x => x.Store);

        if (storeMemberModel is null)
            return Result<Unit>.Failure(StatusCodes.Status403Forbidden, "Current user is not the store owner.");

        var store = Domain.Entities.Store.Load(
            storeMemberModel.Store.Id,
            storeMemberModel.Store.Name,
            storeMemberModel.Store.Slug,
            storeMemberModel.Store.Description,
            storeMemberModel.Store.Tagline,
            storeMemberModel.Store.Location,
            storeMemberModel.Store.ResponseTime,
            storeMemberModel.Store.AvatarUrl,
            storeMemberModel.Store.CoverImageUrl,
            storeMemberModel.Store.IsVerified,
            storeMemberModel.Store.Status,
            storeMemberModel.Store.ShippingPolicy,
            storeMemberModel.Store.ReturnPolicy,
            storeMemberModel.Store.WarrantyPolicy);

        var requestResult = store.RequestVerification();
        if (requestResult.IsFailure)
            return Result<Unit>.Failure(StatusCodes.Status400BadRequest, "Unable to request store verification.");

        storeMemberModel.Store.IsVerified = store.IsVerified;
        storeMemberModel.Store.Status = store.Status;

        return Result<Unit>.Success(Unit.Value);
    }
}
