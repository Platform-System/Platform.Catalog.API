using Platform.Domain.Common;

namespace Platform.Catalog.API.Domain.Entities
{
    public abstract class ProductAsset : Entity
    {
        public Guid ProductId { get; protected set; }
        public Product Product { get; protected set; } = default!;
        public string BlobName { get; protected set; } = default!;
        public string ContainerName { get; protected set; } = default!;
        public string FileName { get; protected set; } = default!;
        public string ContentType { get; protected set; } = default!;
        public long Size { get; protected set; }
        public string? Url { get; protected set; }
        public string? AltText { get; protected set; }

        protected ProductAsset() { }

        protected ProductAsset(
            Guid productId,
            string blobName,
            string containerName,
            string fileName,
            string contentType,
            long size,
            string? altText = null,
            string? url = null)
        {
            ProductId = productId;
            BlobName = blobName;
            ContainerName = containerName;
            FileName = fileName;
            ContentType = contentType;
            Size = size;
            AltText = altText;
            Url = url;
        }

        public void AttachProduct(Product product)
        {
            Product = product;
            ProductId = product.Id;
        }

        public void UpdateAltText(string? altText)
        {
            AltText = altText;
        }

        protected void UpdateAsset(
            string blobName,
            string containerName,
            string fileName,
            string contentType,
            long size,
            string? altText)
        {
            BlobName = blobName;
            ContainerName = containerName;
            FileName = fileName;
            ContentType = contentType;
            Size = size;
            AltText = altText;
            Url = null;
        }

        protected void PublishAsset(string url)
        {
            Url = url;
        }
    }
}
