using Platform.Catalog.API.Domain.Entities;
using Platform.BuildingBlocks.DateTimes;

namespace Platform.Catalog.API.Application.Features.Products.Shared;

public sealed class ProductResponse
{
    public Guid Id { get; init; }
    public string Title { get; init; } = null!;
    public string? CoverImageUrl { get; init; }
    public string Author { get; init; } = null!;
    public long Price { get; init; }
    public ProductKind Kind { get; init; }
    public string[] ProductTypeNames { get; init; } = [];
    public int? Stock { get; init; }
    public string Status { get; init; } = null!;
    public DateTime CreatedAt { get; init; }
}

public static class ProductResponseMapper
{
    public static ProductResponse ToResponse(this Product product, string? coverImageUrl = null)
    {
        return new ProductResponse
        {
            Id = product.Id,
            Title = product.Title,
            CoverImageUrl = coverImageUrl ?? product.CoverImageUrl,
            Author = product.Author,
            Price = product.Price,
            Kind = product is PhysicalProduct ? ProductKind.PhysicalProduct : ProductKind.DigitalProduct,
            ProductTypeNames = product.ProductTypes.Select(x => x.Name).ToArray(),
            Stock = product is PhysicalProduct physical ? physical.Stock : null,
            Status = product.Status.ToString(),
            CreatedAt = product.CreatedAt == default ? Clock.Now : product.CreatedAt
        };
    }
}
