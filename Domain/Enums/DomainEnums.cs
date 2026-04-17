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
        Active,
        Inactive,
        Deleted
    }

    public enum ProductTypeStatus
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
