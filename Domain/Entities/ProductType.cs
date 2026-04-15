using Platform.Domain.Common;

namespace Platform.Catalog.API.Domain.Entities
{
    public class ProductType : AggregateRoot
    {
        public string Name { get; private set; } = null!;

        private readonly List<Product> _products = new();
        public IReadOnlyCollection<Product> Products => _products.AsReadOnly();

        private ProductType() { }

        public ProductType(string name)
        {
            SetName(name);
        }

        public static DomainResult<ProductType> Create(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return DomainResult<ProductType>.Failure(DomainErrors.Validation.Required(nameof(Name)));

            return DomainResult<ProductType>.Success(new ProductType(name));
        }

        public DomainResult UpdateName(string name)
        {
            return SetName(name);
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
    }
}
