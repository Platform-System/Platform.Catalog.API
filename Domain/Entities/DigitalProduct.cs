using Platform.Domain.Common;

namespace Platform.Catalog.API.Domain.Entities
{
    public class DigitalProduct : Product
    {
        private DigitalProduct() { }

        public DigitalProduct(string title, string blobName, string containerName, string author, long price, ProductType productType)
            : base(title, blobName, containerName, author, price, productType)
        {
        }

        public static DomainResult<DigitalProduct> Create(string title, string blobName, string containerName, string author, long price, ProductType productType)
        {
            var product = new DigitalProduct(title, blobName, containerName, author, price, productType);
            return DomainResult<DigitalProduct>.Success(product);
        }
    }
}
