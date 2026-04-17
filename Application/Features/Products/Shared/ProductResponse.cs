using Platform.BuildingBlocks.DateTimes;
using Platform.Catalog.API.Infrastructure.Persistence.Models;
using System.Text.Json;

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
    public static ProductResponse ToResponse(this ProductModel product, string? coverImageUrl = null)
    {
        return new ProductResponse
        {
            Id = product.Id,
            Title = product.Title,
            CoverImageUrl = coverImageUrl ?? product.CoverImageUrl,
            Author = product.Author,
            Price = product.Price,
            Kind = product is PhysicalProductModel ? ProductKind.PhysicalProduct : ProductKind.DigitalProduct,
            ProductTypeNames = product.ProductTypes.Select(x => x.Name).ToArray(),
            Stock = product is PhysicalProductModel physical ? physical.Stock : null,
            Status = product.Status.ToString(),
            CreatedAt = product.CreatedAt == default ? Clock.Now : product.CreatedAt
        };
    }

    public static (string ContainerName, string BlobName)? GetBlobReference(this ProductModel product)
    {
        if (product.AdditionalInfo is null)
            return null;

        if (!product.AdditionalInfo.RootElement.TryGetProperty("blob", out JsonElement blobElement))
            return null;

        if (!blobElement.TryGetProperty("ContainerName", out JsonElement containerElement))
            return null;

        if (!blobElement.TryGetProperty("BlobName", out JsonElement blobNameElement))
            return null;

        var containerName = containerElement.GetString();
        var blobName = blobNameElement.GetString();

        if (string.IsNullOrWhiteSpace(containerName) || string.IsNullOrWhiteSpace(blobName))
            return null;

        return (containerName, blobName);
    }
}
