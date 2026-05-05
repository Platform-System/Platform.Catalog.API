using Platform.BuildingBlocks.DateTimes;
using Platform.Catalog.API.Application.Features.ProductCoverImages.Mappers;
using Platform.Catalog.API.Domain.Entities;
using Platform.Catalog.API.Application.Features.Products.Shared;
using Platform.Catalog.API.Infrastructure.Persistence.Models;

namespace Platform.Catalog.API.Application.Features.Products.Mappers;

public static class ProductResponseMapper
{
    public static ProductResponse ToResponse(this ProductModel product, string? resolvedCoverImageUrl)
    {
        return new ProductResponse
        {
            Id = product.Id,
            Title = product.Title,
            CoverImageUrl = resolvedCoverImageUrl,
            CoverImage = product.CoverImage?.ToResponse(resolvedCoverImageUrl),
            Author = product.Author,
            Price = product.Price,
            CategoryName = product.Category.Name,
            Stock = product.Stock,
            Status = product.Status.ToString(),
            CreatedAt = product.CreatedAt == default ? Clock.Now : product.CreatedAt
        };
    }

    public static ProductResponse ToResponse(this Product product, string? resolvedCoverImageUrl = null)
    {
        return new ProductResponse
        {
            Id = product.Id,
            Title = product.Title,
            CoverImageUrl = resolvedCoverImageUrl ?? product.CoverImage?.Url,
            CoverImage = product.CoverImage?.ToResponse(resolvedCoverImageUrl),
            Author = product.Author,
            Price = product.Price,
            CategoryName = product.GetCategoryName(),
            Stock = product.Stock,
            Status = product.Status.ToString(),
            CreatedAt = product.CreatedAt == default ? Clock.Now : product.CreatedAt
        };
    }
}
