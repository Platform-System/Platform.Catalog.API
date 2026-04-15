using Platform.Catalog.API.Domain.Errors;
using Platform.Domain.Common;

namespace Platform.Catalog.API.Domain.Entities
{
    public class PhysicalProduct : Product
    {
        public int Stock { get; private set; }

        private PhysicalProduct() { }

        public PhysicalProduct(string title, string blobName, string containerName, string author, long price, ProductType productType, int stock)
            : base(title, blobName, containerName, author, price, productType)
        {
            SetStock(stock);
        }

        public static DomainResult<PhysicalProduct> Create(string title, string blobName, string containerName, string author, long price, ProductType productType, int stock)
        {
            if (stock < 0) return DomainResult<PhysicalProduct>.Failure(ProductErrors.InsufficientStock);
            
            var product = new PhysicalProduct(title, blobName, containerName, author, price, productType, stock);
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
    }
}
