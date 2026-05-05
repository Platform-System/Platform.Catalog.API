using Platform.Application.Abstractions.Data;
using Platform.Application.Messaging;
using Platform.BuildingBlocks.Abstractions;
using Platform.BuildingBlocks.Responses;
using Platform.Catalog.API.Application.Features.StoreMembers.Mappers;
using Platform.Catalog.API.Application.Features.Stores.Mappers;
using Platform.Catalog.API.Application.Features.Stores.Shared;
using Platform.Catalog.API.Domain.Entities;
using Platform.Catalog.API.Domain.Enums;
using Platform.Catalog.API.Infrastructure.Persistence.Models;

namespace Platform.Catalog.API.Application.Features.Stores.Commands.Create;

public sealed class CreateStoreHandler : ICommandHandler<CreateStoreCommand, StoreResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserProvider _currentUserProvider;

    public CreateStoreHandler(IUnitOfWork unitOfWork, ICurrentUserProvider currentUserProvider)
    {
        _unitOfWork = unitOfWork;
        _currentUserProvider = currentUserProvider;
    }

    public async Task<Result<StoreResponse>> Handle(CreateStoreCommand command, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(_currentUserProvider.CurrentUserId, out var currentUserId))
            return Result<StoreResponse>.Failure(StatusCodes.Status401Unauthorized, "Current user is invalid.");

        var storeMemberRepository = _unitOfWork.GetRepository<StoreMemberModel>();
        var storeRepository = _unitOfWork.GetRepository<StoreModel>();

        var existingMember = await storeMemberRepository.FindAsync(x => x.UserId == currentUserId, true, cancellationToken);
        if (existingMember is not null)
            return Result<StoreResponse>.Failure(StatusCodes.Status400BadRequest, "Current user already belongs to a store.");

        var slug = command.Request.Name.ToStoreSlug();
        var existingStore = await storeRepository.FindAsync(x => x.Slug == slug, true, cancellationToken);
        if (existingStore is not null)
            return Result<StoreResponse>.Failure(StatusCodes.Status400BadRequest, "Store slug already exists.");

        var store = Store.Create(
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

        var ownerMember = StoreMember.Create(store.Id, currentUserId, StoreMemberRole.Owner);

        await storeRepository.AddAsync(store.ToPersistence(), cancellationToken);
        await storeMemberRepository.AddAsync(ownerMember.ToPersistence(), cancellationToken);

        return Result<StoreResponse>.Success(store.ToResponse());
    }
}
