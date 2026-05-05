using Microsoft.AspNetCore.Http;
using Platform.Application.Abstractions.Data;
using Platform.Application.Messaging;
using Platform.BuildingBlocks.Abstractions;
using Platform.BuildingBlocks.Responses;
using Platform.Catalog.API.Application.Features.Stores.Mappers;
using Platform.Catalog.API.Application.Features.Stores.Shared;
using Platform.Catalog.API.Domain.Enums;
using Platform.Catalog.API.Infrastructure.Persistence.Models;

namespace Platform.Catalog.API.Application.Features.Stores.Commands.Update;

public sealed class UpdateStoreHandler : ICommandHandler<UpdateStoreCommand, StoreResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserProvider _currentUserProvider;

    public UpdateStoreHandler(IUnitOfWork unitOfWork, ICurrentUserProvider currentUserProvider)
    {
        _unitOfWork = unitOfWork;
        _currentUserProvider = currentUserProvider;
    }

    public async Task<Result<StoreResponse>> Handle(UpdateStoreCommand command, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(_currentUserProvider.CurrentUserId, out var currentUserId))
            return Result<StoreResponse>.Failure(StatusCodes.Status401Unauthorized, "Current user is invalid.");

        var storeMemberRepository = _unitOfWork.GetRepository<StoreMemberModel>();
        var storeRepository = _unitOfWork.GetRepository<StoreModel>();

        var ownerMember = await storeMemberRepository.FindAsync(
            x => x.UserId == currentUserId
                && x.Role == StoreMemberRole.Owner
                && x.Status == StoreMemberStatus.Active
                && x.Store.Status != StoreStatus.Deleted,
            true,
            cancellationToken,
            x => x.Store);

        if (ownerMember is null)
            return Result<StoreResponse>.Failure(StatusCodes.Status404NotFound, "Store not found.");

        var slug = command.Request.Name.ToStoreSlug();
        var existingStore = await storeRepository.FindAsync(
            x => x.Slug == slug && x.Id != ownerMember.StoreId,
            true,
            cancellationToken);

        if (existingStore is not null)
            return Result<StoreResponse>.Failure(StatusCodes.Status400BadRequest, "Store slug already exists.");

        var store = ownerMember.Store.ToDomain();
        var updateResult = store.UpdateInfo(
            command.Request.Name,
            slug,
            command.Request.Description,
            command.Request.Tagline,
            command.Request.Location,
            command.Request.ResponseTime,
            command.Request.AvatarUrl,
            command.Request.CoverImageUrl,
            command.Request.ShippingPolicy,
            command.Request.ReturnPolicy,
            command.Request.WarrantyPolicy);

        if (updateResult.IsFailure)
            return Result<StoreResponse>.Failure(StatusCodes.Status400BadRequest, "Unable to update store.");

        ownerMember.Store.ApplyDomainState(store);
        storeRepository.Update(ownerMember.Store);
        return Result<StoreResponse>.Success(store.ToResponse());
    }
}
