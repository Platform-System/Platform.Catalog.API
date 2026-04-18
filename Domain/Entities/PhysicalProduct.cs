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

        public static DomainResult<PhysicalProduct> Create(string title, string author, long price, IEnumerable<ProductType> productTypes, int stock)
        {
            var productTypesList = productTypes?.ToList() ?? [];
            if (stock < 0) return DomainResult<PhysicalProduct>.Failure(ProductErrors.InsufficientStock);
            if (productTypesList.Count == 0) return DomainResult<PhysicalProduct>.Failure(ProductErrors.InvalidType);

            var product = new PhysicalProduct();
            var initializeResult = product.Initialize(title, author, price, productTypesList);
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

        public static PhysicalProduct Load(ProductLoadData loadData, int stock)
        {
            var product = new PhysicalProduct
            {
                Stock = stock
            };

            product.LoadState(
                loadData.Id,
                loadData.Title,
                loadData.Author,
                loadData.Price,
                loadData.Status,
                loadData.PublishedAt,
                loadData.BlobMetadata,
                loadData.CreatedAt,
                loadData.CreatedBy,
                loadData.UpdatedAt,
                loadData.UpdatedBy,
                loadData.IsSoftDeleted,
                loadData.DeletedAt,
                loadData.DeletedBy);

            return product;
        }
    }
}
