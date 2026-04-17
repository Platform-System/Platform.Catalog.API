using Platform.Catalog.API.Domain.Entities;
using Platform.BuildingBlocks.Responses;

namespace Platform.Catalog.API.Application.Features.Products.Shared;

public static class ProductFactory
{
    public static Result<Product> Create(
        ProductKind kind,
        string title,
        string blobName,
        string containerName,
        string author,
        long price,
        IReadOnlyCollection<ProductType> productTypes,
        int? stock = null)
    {
        return kind switch
        {
            ProductKind.DigitalProduct => CreateDigitalProduct(
                title,
                blobName,
                containerName,
                author,
                price,
                productTypes),

            ProductKind.PhysicalProduct => CreatePhysicalProduct(
                title,
                blobName,
                containerName,
                author,
                price,
                productTypes,
                stock),

            _ => Result<Product>.Failure("Unsupported product kind.")
        };
    }

    private static Result<Product> CreateDigitalProduct(
        string title,
        string blobName,
        string containerName,
        string author,
        long price,
        IReadOnlyCollection<ProductType> productTypes)
    {
        var result = DigitalProduct.Create(
            title,
            blobName,
            containerName,
            author,
            price,
            productTypes);

        if (result.IsFailure)
        {
            return Result<Product>.Failure("Unable to create digital product.");
        }

        return Result<Product>.Success(result.Value);
    }

    private static Result<Product> CreatePhysicalProduct(
        string title,
        string blobName,
        string containerName,
        string author,
        long price,
        IReadOnlyCollection<ProductType> productTypes,
        int? stock)
    {
        var result = PhysicalProduct.Create(
            title,
            blobName,
            containerName,
            author,
            price,
            productTypes,
            stock ?? 0);

        if (result.IsFailure)
        {
            return Result<Product>.Failure("Unable to create physical product.");
        }

        return Result<Product>.Success(result.Value);
    }
}
