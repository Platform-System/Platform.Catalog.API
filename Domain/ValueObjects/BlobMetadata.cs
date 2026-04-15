using Platform.Catalog.API.Domain.Enums;

namespace Platform.Catalog.API.Domain.ValueObjects
{
    public class BlobMetadata
    {
        public string BlobName { get; set; } = default!;
        public string ContainerName { get; set; } = default!;
        public BlobStatus Status { get; set; } = BlobStatus.Private;
        public DateTime UploadedAt { get; set; }
    }
}
