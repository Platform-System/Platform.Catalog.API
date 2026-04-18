using Platform.Domain.Common;

namespace Platform.Catalog.API.Domain.Entities
{
    public sealed class ProductCoverImage : Entity
    {
        public Guid ProductId { get; private set; }
        public Product Product { get; private set; } = default!;
        public string BlobName { get; private set; } = default!;
        public string ContainerName { get; private set; } = default!;
        public string? Url { get; private set; }

        private ProductCoverImage() { }

        public ProductCoverImage(Guid productId, string blobName, string containerName, string? url = null)
        {
            ProductId = productId;
            BlobName = blobName;
            ContainerName = containerName;
            Url = url;
        }

        public static ProductCoverImage Load(Guid id, Guid productId, string blobName, string containerName, string? url)
        {
            return new ProductCoverImage(productId, blobName, containerName, url)
            {
                Id = id
            };
        }

        public void AttachProduct(Product product)
        {
            Product = product;
            ProductId = product.Id;
        }

        public void UpdateUrl(string? url)
        {
            Url = url;
        }
    }
}
