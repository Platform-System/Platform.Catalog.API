using Platform.Catalog.API.Domain.Enums;

namespace Platform.Catalog.API.Domain.Entities
{
    public sealed class ProductMedia : ProductAsset
    {
        public MediaType Type { get; private set; }
        public int SortOrder { get; private set; }

        private ProductMedia() { }

        public ProductMedia(
            Guid productId,
            string blobName,
            string containerName,
            string fileName,
            string contentType,
            long size,
            MediaType type,
            int sortOrder = 0,
            string? altText = null,
            string? url = null) : base(productId, blobName, containerName, fileName, contentType, size, altText, url)
        {
            Type = type;
            SortOrder = sortOrder;
        }

        public static ProductMedia Load(
            Guid id,
            Guid productId,
            string blobName,
            string containerName,
            string fileName,
            string contentType,
            long size,
            MediaType type,
            int sortOrder,
            string? altText,
            string? url)
        {
            return new ProductMedia(productId, blobName, containerName, fileName, contentType, size, type, sortOrder, altText, url)
            {
                Id = id
            };
        }

        public void UpdateMetadata(string blobName, string containerName, string fileName, string contentType, long size)
        {
            UpdateAsset(blobName, containerName, fileName, contentType, size, AltText);
        }

        public void Publish(string url)
        {
            PublishAsset(url);
        }
    }
}
