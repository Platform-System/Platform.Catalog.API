using Platform.Domain.Common;
using Platform.Catalog.API.Domain.Enums;

namespace Platform.Catalog.API.Domain.Entities
{
    public class ProductType : AggregateRoot
    {
        public string Name { get; private set; } = null!;
        public ProductTypeStatus Status { get; private set; }

        private readonly List<Product> _products = new();
        public IReadOnlyCollection<Product> Products => _products.AsReadOnly();

        private ProductType() { }

        public ProductType(string name)
        {
            SetName(name);
            Status = ProductTypeStatus.Active;
        }

        public static DomainResult<ProductType> Create(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return DomainResult<ProductType>.Failure(DomainErrors.Validation.Required(nameof(Name)));

            return DomainResult<ProductType>.Success(new ProductType(name));
        }

        public DomainResult UpdateName(string name)
        {
            if (Status != ProductTypeStatus.Active)
                return DomainResult.Failure(DomainErrors.Global.NotFound(nameof(ProductType), Id));

            return SetName(name);
        }

        public DomainResult Delete()
        {
            if (Status == ProductTypeStatus.Deleted)
                return DomainResult.Success();

            Status = ProductTypeStatus.Deleted;
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

        public static ProductType Load(Guid id, string name, ProductTypeStatus status)
        {
            var productType = new ProductType
            {
                Id = id,
                Name = name,
                Status = status
            };

            return productType;
        }
    }
}
