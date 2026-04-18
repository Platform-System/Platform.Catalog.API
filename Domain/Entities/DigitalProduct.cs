using Platform.Catalog.API.Domain.Errors;
using Platform.Catalog.API.Domain.Enums;
using Platform.Catalog.API.Domain.ValueObjects;
using Platform.Domain.Common;

namespace Platform.Catalog.API.Domain.Entities
{
    public class DigitalProduct : Product
    {
        private DigitalProduct() { }

        public static DomainResult<DigitalProduct> Create(string title, string author, long price, IEnumerable<ProductType> productTypes)
        {
            var productTypesList = productTypes?.ToList() ?? [];
            if (productTypesList.Count == 0)
                return DomainResult<DigitalProduct>.Failure(ProductErrors.InvalidType);

            var product = new DigitalProduct();
            var initializeResult = product.Initialize(title, author, price, productTypesList);
            if (initializeResult.IsFailure)
                return DomainResult<DigitalProduct>.Failure(initializeResult.Error);

            return DomainResult<DigitalProduct>.Success(product);
        }

        public static DigitalProduct Load(ProductLoadData loadData)
        {
            var product = new DigitalProduct();
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
