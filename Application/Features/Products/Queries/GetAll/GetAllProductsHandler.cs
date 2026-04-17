using Microsoft.EntityFrameworkCore;
using Platform.Application.Abstractions.Data;
using Platform.Application.Messaging;
using Platform.BuildingBlocks.Responses;
using Platform.Catalog.API.Application.Features.Products.Shared;
using Platform.Catalog.API.Application.Mappers;
using Platform.Catalog.API.Domain.Enums;
using Platform.Catalog.API.Infrastructure.Persistence.Models;

namespace Platform.Catalog.API.Application.Features.Products.Queries.GetAll;

public sealed class GetAllProductsHandler : IQueryHandler<GetAllProductsQuery, PagedResult<ProductResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllProductsHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PagedResult<ProductResponse>>> Handle(GetAllProductsQuery query, CancellationToken cancellationToken)
    {
        IQueryable<ProductModel> productQuery = _unitOfWork
            .GetRepository<ProductModel>()
            .GetQueryable()
            .AsNoTracking()
            .Include(x => x.ProductTypes)
            .Where(x => x.Status == ProductStatus.Active);

        if (!string.IsNullOrWhiteSpace(query.Request.ProductTypeName))
        {
            var productTypeName = query.Request.ProductTypeName.Trim();
            productQuery = productQuery.Where(x => x.ProductTypes.Any(t => t.Name.Contains(productTypeName)));
        }

        if (!string.IsNullOrWhiteSpace(query.Request.Title))
        {
            var title = query.Request.Title.Trim();
            productQuery = productQuery.Where(x => x.Title.Contains(title));
        }

        if (query.Request.Kind == ProductKind.DigitalProduct)
        {
            productQuery = productQuery.Where(x => x is DigitalProductModel);
        }
        else if (query.Request.Kind == ProductKind.PhysicalProduct)
        {
            productQuery = productQuery.Where(x => x is PhysicalProductModel);
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
            Items = items.Select(x => x.ToDomain().ToResponse()).ToList()
        };

        return Result<PagedResult<ProductResponse>>.Success(result);
    }
}
