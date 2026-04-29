using Platform.Domain.Common;
using Platform.Catalog.API.Domain.Enums;

namespace Platform.Catalog.API.Domain.Entities
{
    public class Category : AggregateRoot
    {
        public string Name { get; private set; } = null!;
        public CategoryStatus Status { get; private set; }

        private readonly List<Product> _products = new();
        public IReadOnlyCollection<Product> Products => _products.AsReadOnly();

        private Category() { }

        public Category(string name)
        {
            SetName(name);
            Status = CategoryStatus.Active;
        }

        public static DomainResult<Category> Create(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return DomainResult<Category>.Failure(DomainErrors.Validation.Required(nameof(Name)));

            return DomainResult<Category>.Success(new Category(name));
        }

        public DomainResult UpdateName(string name)
        {
            if (Status != CategoryStatus.Active)
                return DomainResult.Failure(DomainErrors.Global.NotFound(nameof(Category), Id));

            return SetName(name);
        }

        public DomainResult Delete()
        {
            if (Status == CategoryStatus.Deleted)
                return DomainResult.Success();

            Status = CategoryStatus.Deleted;
            return DomainResult.Success();
        }

        private DomainResult SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return DomainResult.Failure(DomainErrors.Validation.Required(nameof(Name)));

            Name = name.Trim();
            return DomainResult.Success();
        }

        public void AddProduct(Product product)
        {
            if (product == null) return;
            if (_products.Contains(product)) return;

            _products.Add(product);
        }

        public void RemoveProduct(Product product)
        {
            if (product == null) return;
            _products.Remove(product);
        }

        public static Category Load(Guid id, string name, CategoryStatus status)
        {
            var category = new Category
            {
                Id = id,
                Name = name,
                Status = status
            };

            return category;
        }
    }
}
