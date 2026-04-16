using Platform.Catalog.API.Domain.Errors;
using Platform.Catalog.API.Domain.Enums;
using Platform.Catalog.API.Domain.ValueObjects;
using Platform.Domain.Common;

namespace Platform.Catalog.API.Domain.Entities
{
    public class PhysicalProduct : Product
    {
        public int Stock { get; private set; }

        private PhysicalProduct() { }

        public static DomainResult<PhysicalProduct> Create(string title, string blobName, string containerName, string author, long price, ProductType productType, int stock)
        {
            if (stock < 0) return DomainResult<PhysicalProduct>.Failure(ProductErrors.InsufficientStock);
            if (productType is null) return DomainResult<PhysicalProduct>.Failure(ProductErrors.InvalidType);

            var product = new PhysicalProduct();
            var initializeResult = product.Initialize(title, blobName, containerName, author, price, [productType]);
            if (initializeResult.IsFailure)
                return DomainResult<PhysicalProduct>.Failure(initializeResult.Error);

            var stockResult = product.SetStock(stock);
            if (stockResult.IsFailure)
                return DomainResult<PhysicalProduct>.Failure(stockResult.Error);

            return DomainResult<PhysicalProduct>.Success(product);
        }

        public DomainResult UpdateStock(int quantity)
        {
            return SetStock(quantity);
        }

        private DomainResult SetStock(int quantity)
        {
            if (quantity < 0)
                return DomainResult.Failure(ProductErrors.InsufficientStock);

            Stock = quantity;
            return DomainResult.Success();
        }

        public DomainResult Restock(int quantity)
        {
            if (quantity <= 0) 
                return DomainResult.Failure(DomainErrors.Validation.InvalidInput);

            Stock += quantity;
            return DomainResult.Success();
        }

        public DomainResult ReduceStock(int quantity)
        {
            if (quantity <= 0)
                return DomainResult.Failure(DomainErrors.Validation.InvalidInput);

            if (Stock < quantity)
                return DomainResult.Failure(ProductErrors.InsufficientStock);

            Stock -= quantity;
            return DomainResult.Success();
        }

        public static PhysicalProduct Load(
            Guid id,
            string title,
            string? coverImageUrl,
            string author,
            long price,
            ProductStatus status,
            DateTime? publishedAt,
            BlobMetadata? blobMetadata,
            int stock,
            DateTime createdAt,
            string? createdBy,
            DateTime? updatedAt,
            string? updatedBy,
            bool isSoftDeleted,
            DateTime? deletedAt,
            string? deletedBy)
        {
            var product = new PhysicalProduct
            {
                Stock = stock
            };

            product.LoadState(id, title, coverImageUrl, author, price, status, publishedAt, blobMetadata, createdAt, createdBy, updatedAt, updatedBy, isSoftDeleted, deletedAt, deletedBy);
            return product;
        }
    }
}
