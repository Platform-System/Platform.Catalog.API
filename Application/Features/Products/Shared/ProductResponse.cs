using Platform.BuildingBlocks.DateTimes;
using Platform.Application.Abstractions.Storage;
using Platform.Catalog.API.Infrastructure.Persistence.Models;

using Platform.SharedKernel.Enums;

namespace Platform.Catalog.API.Application.Features.Products.Shared;

public sealed class ProductResponse
{
    public Guid Id { get; init; }
    public string Title { get; init; } = null!;
    public string? CoverImageUrl { get; init; }
    public ProductCoverImageResponse? CoverImage { get; init; }
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
    public static ProductResponse ToResponse(this ProductModel product, IBlobService? blobService = null)
    {
        return new ProductResponse
        {
            Id = product.Id,
            Title = product.Title,
            CoverImageUrl = product.CoverImage is null
                ? null
                : string.IsNullOrWhiteSpace(product.CoverImage.Url)
                    ? blobService?.GenerateReadSasUrl(product.CoverImage.ContainerName, product.CoverImage.BlobName)
                    : product.CoverImage.Url,
            CoverImage = product.CoverImage is null
                ? null
                : ProductCoverImageResponse.FromModel(product.CoverImage),
            Author = product.Author,
            Price = product.Price,
            Kind = product is PhysicalProductModel ? ProductKind.PhysicalProduct : ProductKind.DigitalProduct,
            ProductTypeNames = product.ProductTypes.Select(x => x.Name).ToArray(),
            Stock = product is PhysicalProductModel physical ? physical.Stock : null,
            Status = product.Status.ToString(),
            CreatedAt = product.CreatedAt == default ? Clock.Now : product.CreatedAt
        };
    }

}
