using Microsoft.AspNetCore.Http;
using Platform.Application.Abstractions.Data;
using Platform.Application.Abstractions.Storage;
using Platform.Application.Messaging;
using Platform.BuildingBlocks.Responses;
using Platform.Catalog.API.Application.Features.Products.Mappers;
using Platform.Catalog.API.Application.Features.Products.Shared;
using Platform.Catalog.API.Domain.Enums;
using Platform.Catalog.API.Infrastructure.Persistence.Models;

namespace Platform.Catalog.API.Application.Features.Stores.Queries.GetProductsBySlug;

public sealed class GetStoreProductsBySlugHandler : IQueryHandler<GetStoreProductsBySlugQuery, PagedResult<ProductResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBlobService _blobService;

    public GetStoreProductsBySlugHandler(IUnitOfWork unitOfWork, IBlobService blobService)
    {
        _unitOfWork = unitOfWork;
        _blobService = blobService;
    }

    public async Task<Result<PagedResult<ProductResponse>>> Handle(GetStoreProductsBySlugQuery query, CancellationToken cancellationToken)
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
            return Result<PagedResult<ProductResponse>>.Failure(StatusCodes.Status404NotFound, "Store not found.");

        var products = await _unitOfWork
            .GetRepository<ProductModel>()
            .GetPagedAsync(
                query.Page,
                query.PageSize,
                x => x.StoreId == store.Id
                    && x.Status == ProductStatus.Active,
                x => x.CreatedAt,
                true,
                cancellationToken,
                x => x.Category,
                x => x.CoverImage!);

        var pagedResult = new PagedResult<ProductResponse>
        {
            Items = products.Items.Select(x => x.ToResponse(x.ResolveCoverImageUrl(_blobService))).ToList(),
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = products.TotalCount
        };

        return Result<PagedResult<ProductResponse>>.Success(pagedResult);
    }
}
