using Platform.Domain.Common;

namespace Platform.Catalog.API.Domain.Entities
{
    public sealed class ProductCoverImage : Entity
    {
        public Guid ProductId { get; private set; }
        public Product Product { get; private set; } = default!;
        public string BlobName { get; private set; } = default!;
        public string ContainerName { get; private set; } = default!;
        public string FileName { get; private set; } = default!;
        public string ContentType { get; private set; } = default!;
        public long Size { get; private set; }
        public string? Url { get; private set; }

        private ProductCoverImage() { }

        public ProductCoverImage(
            Guid productId,
            string blobName,
            string containerName,
            string fileName,
            string contentType,
            long size,
            string? url = null)
        {
            ProductId = productId;
            BlobName = blobName;
            ContainerName = containerName;
            FileName = fileName;
            ContentType = contentType;
            Size = size;
            Url = url;
        }

        public static ProductCoverImage Load(
            Guid id,
            Guid productId,
            string blobName,
            string containerName,
            string fileName,
            string contentType,
            long size,
            string? url)
        {
            return new ProductCoverImage(productId, blobName, containerName, fileName, contentType, size, url)
            {
                Id = id
            };
        }

        public void AttachProduct(Product product)
        {
            Product = product;
            ProductId = product.Id;
        }

        public void UpdateMetadata(string blobName, string containerName, string fileName, string contentType, long size)
        {
            BlobName = blobName;
            ContainerName = containerName;
            FileName = fileName;
            ContentType = contentType;
            Size = size;
            Url = null;
        }

        public void Publish(string url)
        {
            Url = url;
        }
    }
}
