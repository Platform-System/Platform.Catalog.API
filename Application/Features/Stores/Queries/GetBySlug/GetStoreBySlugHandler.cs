using Platform.Application.Abstractions.Data;
using Platform.Application.Messaging;
using Platform.BuildingBlocks.Responses;
using Platform.Catalog.API.Application.Features.Stores.Mappers;
using Platform.Catalog.API.Application.Features.Stores.Shared;
using Platform.Catalog.API.Domain.Enums;
using Platform.Catalog.API.Infrastructure.Persistence.Models;

namespace Platform.Catalog.API.Application.Features.Stores.Queries.GetBySlug;

public sealed class GetStoreBySlugHandler : IQueryHandler<GetStoreBySlugQuery, StoreResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetStoreBySlugHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<StoreResponse>> Handle(GetStoreBySlugQuery query, CancellationToken cancellationToken)
    {
        var slug = query.Slug.Trim();
        var store = await _unitOfWork
            .GetRepository<StoreModel>()
            .FindAsync(
                x => x.Slug == slug
                    && x.Status != StoreStatus.Deleted,
                true,
                cancellationToken);

        if (store is null)
            return Result<StoreResponse>.Failure(StatusCodes.Status404NotFound, "Store not found.");

        return Result<StoreResponse>.Success(store.ToResponse());
    }
}
