using Platform.Application.Abstractions.Data;
using Platform.Application.Abstractions.Storage;
using Platform.Application.Messaging;
using Platform.BuildingBlocks.Responses;
using Platform.Catalog.API.Application.Abstractions.Stores;
using Platform.Catalog.API.Application.Features.ProductCoverImages.Shared;
using Platform.Catalog.API.Application.Features.Products.Mappers;
using Platform.Catalog.API.Application.Features.Products.Shared;
using Platform.Catalog.API.Domain.Enums;
using Platform.Catalog.API.Infrastructure.Persistence.Models;

namespace Platform.Catalog.API.Application.Features.Products.Queries.GetProductsByStoreSlug;

public sealed class GetProductsByStoreSlugHandler : IQueryHandler<GetProductsByStoreSlugQuery, PagedResult<ProductResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBlobService _blobService;
    private readonly IStoreReadService _storeReadService;

    public GetProductsByStoreSlugHandler(IUnitOfWork unitOfWork, IBlobService blobService, IStoreReadService storeReadService)
    {
        _unitOfWork = unitOfWork;
        _blobService = blobService;
        _storeReadService = storeReadService;
    }

    public async Task<Result<PagedResult<ProductResponse>>> Handle(GetProductsByStoreSlugQuery query, CancellationToken cancellationToken)
    {
        var storeId = await _storeReadService.GetStoreIdBySlugAsync(query.Slug, cancellationToken);
        if (storeId is null)
            return Result<PagedResult<ProductResponse>>.Failure(StatusCodes.Status404NotFound, "Store not found.");

        var products = await _unitOfWork
            .GetRepository<ProductModel>()
            .GetPagedAsync(
                query.Page,
                query.PageSize,
                x => x.StoreId == storeId.Value
                    && x.Status == ProductStatus.Active,
                x => x.CreatedAt,
                true,
                cancellationToken,
                x => x.Category,
                x => x.CoverImage!);

        return Result<PagedResult<ProductResponse>>.Success(new PagedResult<ProductResponse>
        {
            Items = products.Items.Select(x => x.ToResponse(x.ResolveCoverImageUrl(_blobService))).ToList(),
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = products.TotalCount
        });
    }
}
