using Platform.Domain.Common;

namespace Platform.Catalog.API.Domain.Errors
{
    public static class ProductErrors
    {
        public static Error InsufficientStock => new("Product.InsufficientStock", "Not enough stock available for this operation.");
        public static Error InvalidPrice => new("Product.InvalidPrice", "Product price cannot be negative.");
        public static Error AlreadyDeleted => new("Product.AlreadyDeleted", "Action cannot be performed on a deleted product.");
        public static Error NameTooShort => new("Product.NameTooShort", "Product name must be at least 3 characters long.");
        public static Error InvalidType => new("Product.InvalidType", "The specified category is invalid.");
    }
}
