namespace Platform.Catalog.API.Domain.Enums
{
    public enum BlobStatus
    {
        Private,
        Public
    }

    public enum ProductStatus
    {
        Draft,
        PendingOwnerReview,
        PendingAdminReview,
        Active,
        Inactive,
        Deleted
    }

    public enum CategoryStatus
    {
        Active,
        Deleted
    }

    public enum StoreStatus
    {
        Draft,
        PendingVerification,
        Active,
        Suspended,
        Deleted
    }

    public enum StoreMemberRole
    {
        Owner,
        Manager,
        Staff
    }

    public enum StoreMemberStatus
    {
        Active,
        Invited,
        Removed
    }

    public enum MediaType
    {
        Image,
        Video,
        Document,
        Other
    }
}
