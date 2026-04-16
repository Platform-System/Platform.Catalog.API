using Platform.Catalog.API.Domain.Errors;
using Platform.Catalog.API.Domain.Enums;
using Platform.Catalog.API.Domain.ValueObjects;
using Platform.Domain.Common;

namespace Platform.Catalog.API.Domain.Entities
{
    public class DigitalProduct : Product
    {
        private DigitalProduct() { }

        public static DomainResult<DigitalProduct> Create(string title, string blobName, string containerName, string author, long price, ProductType productType)
        {
            if (productType is null)
                return DomainResult<DigitalProduct>.Failure(ProductErrors.InvalidType);

            var product = new DigitalProduct();
            var initializeResult = product.Initialize(title, blobName, containerName, author, price, [productType]);
            if (initializeResult.IsFailure)
                return DomainResult<DigitalProduct>.Failure(initializeResult.Error);

            return DomainResult<DigitalProduct>.Success(product);
        }

        public static DigitalProduct Load(
            Guid id,
            string title,
            string? coverImageUrl,
            string author,
            long price,
            ProductStatus status,
            DateTime? publishedAt,
            BlobMetadata? blobMetadata,
            DateTime createdAt,
            string? createdBy,
            DateTime? updatedAt,
            string? updatedBy,
            bool isSoftDeleted,
            DateTime? deletedAt,
            string? deletedBy)
        {
            var product = new DigitalProduct();
            product.LoadState(id, title, coverImageUrl, author, price, status, publishedAt, blobMetadata, createdAt, createdBy, updatedAt, updatedBy, isSoftDeleted, deletedAt, deletedBy);
            return product;
        }
    }
}
