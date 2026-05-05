using Platform.Application.Abstractions.Data;
using Platform.Application.Messaging;
using Platform.BuildingBlocks.Abstractions;
using Platform.BuildingBlocks.Responses;
using Platform.Catalog.API.Application.Features.Stores.Mappers;
using Platform.Catalog.API.Application.Features.Stores.Shared;
using Platform.Catalog.API.Domain.Enums;
using Platform.Catalog.API.Infrastructure.Persistence.Models;

namespace Platform.Catalog.API.Application.Features.Stores.Queries.GetStoreByUser;

public sealed class GetStoreByUserHandler : IQueryHandler<GetStoreByUserQuery, StoreResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserProvider _currentUserProvider;

    public GetStoreByUserHandler(IUnitOfWork unitOfWork, ICurrentUserProvider currentUserProvider)
    {
        _unitOfWork = unitOfWork;
        _currentUserProvider = currentUserProvider;
    }

    public async Task<Result<StoreResponse>> Handle(GetStoreByUserQuery query, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(_currentUserProvider.CurrentUserId, out var currentUserId))
            return Result<StoreResponse>.Failure(StatusCodes.Status401Unauthorized, "Current user is invalid.");

        var storeMember = await _unitOfWork
            .GetRepository<StoreMemberModel>()
            .FindAsync(
                x => x.UserId == currentUserId
                    && x.Status == StoreMemberStatus.Active
                    && x.Store.Status != StoreStatus.Deleted,
                true,
                cancellationToken,
                x => x.Store);

        if (storeMember is null)
            return Result<StoreResponse>.Failure(StatusCodes.Status404NotFound, "Store not found.");

        return Result<StoreResponse>.Success(storeMember.Store.ToResponse());
    }
}
