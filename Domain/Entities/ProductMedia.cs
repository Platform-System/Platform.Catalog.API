using Platform.Catalog.API.Domain.Enums;
using Platform.Domain.Common;

namespace Platform.Catalog.API.Domain.Entities
{
    public class ProductMedia : AggregateRoot
    {
        public string FileName { get; private set; } = default!;
        public string Url { get; private set; } = default!;
        public string ContentType { get; private set; } = default!;
        public long Size { get; private set; }
        public string? AltText { get; private set; }
        public MediaType Type { get; private set; }
        public Guid ProductId { get; private set; }
        public Product Product { get; private set; } = default!;
        public int SortOrder { get; private set; }

        private ProductMedia() { }

        public ProductMedia(string fileName, string url, string contentType, long size, MediaType type, Guid productId, int sortOrder = 0, string? altText = null)
        {
            FileName = fileName;
            Url = url;
            ContentType = contentType;
            Size = size;
            Type = type;
            ProductId = productId;
            SortOrder = sortOrder;
            AltText = altText;
        }
    }
}
