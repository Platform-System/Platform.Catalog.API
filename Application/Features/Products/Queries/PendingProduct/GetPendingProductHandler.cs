using Microsoft.EntityFrameworkCore;
using Platform.Application.Abstractions.Data;
using Platform.Application.Abstractions.Storage;
using Platform.Application.Messaging;
using Platform.BuildingBlocks.Responses;
using Platform.Catalog.API.Application.Features.Products.Shared;
using Platform.Catalog.API.Application.Mappers;
using Platform.Catalog.API.Domain.Enums;
using Platform.Catalog.API.Infrastructure.Persistence.Models;

namespace Platform.Catalog.API.Application.Features.Products.Queries.PendingProduct;

public sealed class GetPendingProductHandler : IQueryHandler<GetPendingProductQuery, PagedResult<ProductResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBlobService _blobService;

    public GetPendingProductHandler(IUnitOfWork unitOfWork, IBlobService blobService)
    {
        _unitOfWork = unitOfWork;
        _blobService = blobService;
    }

    public async Task<Result<PagedResult<ProductResponse>>> Handle(GetPendingProductQuery query, CancellationToken cancellationToken)
    {
        IQueryable<ProductModel> productQuery = _unitOfWork
            .GetRepository<ProductModel>()
            .GetQueryable()
            .AsNoTracking()
            .Include(x => x.ProductTypes)
            .Where(x => x.Status == ProductStatus.Draft);

        var totalCount = await productQuery.CountAsync(cancellationToken);

        if (totalCount == 0)
            return Result<PagedResult<ProductResponse>>.Failure("No pending products found.");

        var productModels = await productQuery
            .OrderByDescending(x => x.CreatedAt)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);

        var items = new List<ProductResponse>(productModels.Count);

        foreach (var productModel in productModels)
        {
            var product = productModel.ToDomain();
            var blob = product.GetBlob();
            string? coverImageUrl = null;

            if (blob is not null)
            {
                coverImageUrl = _blobService.GenerateReadSasUrl(blob.ContainerName, blob.BlobName);
            }

            items.Add(product.ToResponse(coverImageUrl));
        }

        var pagedResult = new PagedResult<ProductResponse>
        {
            Items = items,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = totalCount
        };

        return Result<PagedResult<ProductResponse>>.Success(pagedResult);
    }
}
