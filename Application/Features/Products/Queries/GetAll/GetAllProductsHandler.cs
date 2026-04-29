using Microsoft.EntityFrameworkCore;
using Platform.Application.Abstractions.Storage;
using Platform.Application.Abstractions.Data;
using Platform.Application.Messaging;
using Platform.BuildingBlocks.Responses;
using Platform.Catalog.API.Application.Features.Products.Shared;
using Platform.Catalog.API.Domain.Enums;
using Platform.Catalog.API.Infrastructure.Persistence.Models;

namespace Platform.Catalog.API.Application.Features.Products.Queries.GetAll;

public sealed class GetAllProductsHandler : IQueryHandler<GetAllProductsQuery, PagedResult<ProductResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBlobService _blobService;

    public GetAllProductsHandler(IUnitOfWork unitOfWork, IBlobService blobService)
    {
        _unitOfWork = unitOfWork;
        _blobService = blobService;
    }

    public async Task<Result<PagedResult<ProductResponse>>> Handle(GetAllProductsQuery query, CancellationToken cancellationToken)
    {
        IQueryable<ProductModel> productQuery = _unitOfWork
            .GetRepository<ProductModel>()
            .GetQueryable()
            .AsNoTracking()
            .Include(x => x.Category)
            .Include(x => x.CoverImage)
            .Where(x => x.Status == ProductStatus.Active);

        if (!string.IsNullOrWhiteSpace(query.Request.CategoryName))
        {
            var categoryName = query.Request.CategoryName.Trim();
            productQuery = productQuery.Where(x => x.Category.Name.Contains(categoryName));
        }

        if (!string.IsNullOrWhiteSpace(query.Request.Title))
        {
            var title = query.Request.Title.Trim();
            productQuery = productQuery.Where(x => x.Title.Contains(title));
        }

        var totalCount = await productQuery.CountAsync(cancellationToken);

        var items = await productQuery
            .OrderByDescending(x => x.CreatedAt)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);

        var result = new PagedResult<ProductResponse>
        {
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = totalCount,
            Items = items.Select(x => x.ToResponse(_blobService)).ToList()
        };

        return Result<PagedResult<ProductResponse>>.Success(result);
    }
}
