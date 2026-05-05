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
