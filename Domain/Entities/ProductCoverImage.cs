namespace Platform.Catalog.API.Domain.Entities
{
    public sealed class ProductCoverImage : ProductAsset
    {
        private ProductCoverImage() { }

        public ProductCoverImage(
            Guid productId,
            string blobName,
            string containerName,
            string fileName,
            string contentType,
            long size,
            string? altText = null,
            string? url = null) : base(productId, blobName, containerName, fileName, contentType, size, altText, url)
        {
        }

        public static ProductCoverImage Load(
            Guid id,
            Guid productId,
            string blobName,
            string containerName,
            string fileName,
            string contentType,
            long size,
            string? altText,
            string? url)
        {
            return new ProductCoverImage(productId, blobName, containerName, fileName, contentType, size, altText, url)
            {
                Id = id
            };
        }

        public void UpdateMetadata(string blobName, string containerName, string fileName, string contentType, long size, string? altText = null)
        {
            UpdateAsset(blobName, containerName, fileName, contentType, size, altText);
        }

        public void Publish(string url)
        {
            PublishAsset(url);
        }
    }
}
