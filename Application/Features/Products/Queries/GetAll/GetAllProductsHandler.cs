using Platform.Application.Abstractions.Storage;
using Platform.Application.Abstractions.Data;
using Platform.Application.Messaging;
using Platform.BuildingBlocks.Responses;
using Platform.Catalog.API.Application.Features.ProductCoverImages.Responses;
using Platform.Catalog.API.Application.Features.ProductCoverImages.Shared;
using Platform.Catalog.API.Application.Features.Products.Mappers;
using Platform.Catalog.API.Application.Features.Products.Responses;
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
        var categoryName = query.Request.CategoryName?.Trim();
        var title = query.Request.Title?.Trim();

        var products = await _unitOfWork
            .GetRepository<ProductModel>()
            .GetPagedAsync(
                query.Page,
                query.PageSize,
                x => x.Status == ProductStatus.Active
                    && (string.IsNullOrWhiteSpace(categoryName) || x.Category.Name.Contains(categoryName))
                    && (string.IsNullOrWhiteSpace(title) || x.Title.Contains(title)),
                x => x.CreatedAt,
                true,
                cancellationToken,
                x => x.Category,
                x => x.CoverImage!);

        var result = new PagedResult<ProductResponse>
        {
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = products.TotalCount,
            Items = products.Items
                .Select(x => x.ToResponse(x.ResolveCoverImageUrl(_blobService)))
                .ToList()
        };

        return Result<PagedResult<ProductResponse>>.Success(result);
    }
}
