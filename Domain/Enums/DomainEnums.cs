namespace Platform.Catalog.API.Domain.Enums
{
    public enum BlobStatus
    {
        Private,
        Public
    }

    public enum ProductStatus
    {
        Draft = 0,
        PendingOwnerReview = 1,
        Active = 3,
        Inactive = 4,
        Deleted = 5
    }

    public enum CategoryStatus
    {
        Active,
        Deleted
    }

    public enum MediaType
    {
        Image,
        Video,
        Document,
        Other
    }
}
