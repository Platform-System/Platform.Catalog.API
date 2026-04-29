using Platform.BuildingBlocks.DateTimes;
using Platform.Application.Abstractions.Storage;
using Platform.Catalog.API.Infrastructure.Persistence.Models;

namespace Platform.Catalog.API.Application.Features.Products.Shared;

public sealed class ProductResponse
{
    public Guid Id { get; init; }
    public string Title { get; init; } = null!;
    public string? CoverImageUrl { get; init; }
    public ProductCoverImageResponse? CoverImage { get; init; }
    public string Author { get; init; } = null!;
    public long Price { get; init; }
    public string CategoryName { get; init; } = null!;
    public int Stock { get; init; }
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
            CategoryName = product.Category.Name,
            Stock = product.Stock,
            Status = product.Status.ToString(),
            CreatedAt = product.CreatedAt == default ? Clock.Now : product.CreatedAt
        };
    }

}
