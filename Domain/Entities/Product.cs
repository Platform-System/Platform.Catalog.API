using Platform.BuildingBlocks.DateTimes;
using Platform.Catalog.API.Domain.Enums;
using Platform.Catalog.API.Domain.Errors;
using Platform.Catalog.API.Domain.Extensions;
using Platform.Catalog.API.Domain.ValueObjects;
using Platform.Domain.Common;
using System.Text.Json;

namespace Platform.Catalog.API.Domain.Entities
{
    public abstract class Product : AggregateRoot
    {
        public string Title { get; protected set; } = null!;
        public string Author { get; protected set; } = null!;
        public long Price { get; protected set; }
        public ProductStatus Status { get; protected set; }
        public DateTime? PublishedAt { get; protected set; }
        public JsonDocument? AdditionalInfo { get; protected set; }

        private readonly List<ProductType> _productTypes = new();
        public IReadOnlyCollection<ProductType> ProductTypes => _productTypes.AsReadOnly();

        private readonly List<ProductMedia> _mediaFiles = new();
        public IReadOnlyCollection<ProductMedia> MediaFiles => _mediaFiles.AsReadOnly();
        public ProductCoverImage? CoverImage { get; private set; }

        protected Product() { }

        protected DomainResult Initialize(string title, string author, long price, IEnumerable<ProductType> productTypes)
        {
            var productTypesList = productTypes?.ToList() ?? [];
            if (productTypesList.Count == 0)
                return DomainResult.Failure(ProductErrors.InvalidType);

            var infoResult = SetInfo(title, author, price);
            if (infoResult.IsFailure)
                return infoResult;

            foreach (var type in productTypesList)
            {
                AddProductType(type);
            }

            return DomainResult.Success();
        }

        public DomainResult UpdateInfo(string title, string author, long price, List<ProductType> newTypes)
        {
            if (newTypes == null || !newTypes.Any())
                return DomainResult.Failure(ProductErrors.InvalidType);

            var validationResult = ValidateInfo(title, author, price);
            if (validationResult.IsFailure)
                return validationResult;

            Title = title.Trim();
            Author = author.Trim();
            Price = price;

            var currentTypes = _productTypes.ToList();
            foreach (var type in currentTypes) RemoveProductType(type);
            foreach (var type in newTypes) AddProductType(type);

            return DomainResult.Success();
        }

        protected DomainResult SetInfo(string title, string author, long price)
        {
            var validationResult = ValidateInfo(title, author, price);
            if (validationResult.IsFailure)
                return validationResult;

            Title = title.Trim();
            Author = author.Trim();
            Price = price;

            return DomainResult.Success();
        }

        private static DomainResult ValidateInfo(string title, string author, long price)
        {
            if (string.IsNullOrWhiteSpace(title)) return DomainResult.Failure(DomainErrors.Validation.Required(nameof(Title)));
            if (string.IsNullOrWhiteSpace(author)) return DomainResult.Failure(DomainErrors.Validation.Required(nameof(Author)));
            if (price < 0) return DomainResult.Failure(ProductErrors.InvalidPrice);

            return DomainResult.Success();
        }

        public void AddProductType(ProductType productType)
        {
            if (productType == null) return;
            if (_productTypes.Any(x => x.Id == productType.Id)) return;

            _productTypes.Add(productType);
            productType.AddProduct(this);
        }

        public void RemoveProductType(ProductType productType)
        {
            var existing = _productTypes.FirstOrDefault(x => x.Id == productType.Id);
            if (existing == null) return;

            _productTypes.Remove(existing);
            existing.RemoveProduct(this);
        }

        public string GetProductTypeNames() => string.Join(", ", _productTypes.Select(x => x.Name));

        public void SetBlob(BlobMetadata blob) => AdditionalInfo = AdditionalInfo.Set(AdditionalInfoExtensions.BlobKey, blob);
        public BlobMetadata? GetBlob() => AdditionalInfo.Get<BlobMetadata>(AdditionalInfoExtensions.BlobKey);

        public DomainResult PublishBlob(string publicUrl)
        {
            if (string.IsNullOrWhiteSpace(publicUrl)) return DomainResult.Failure(DomainErrors.Validation.Required("PublicUrl"));
            var blob = GetBlob();
            if (blob == null) return DomainResult.Failure(DomainErrors.Global.NotFound("Blob", Id));

            blob.Status = BlobStatus.Public;
            SetBlob(blob);
            return DomainResult.Success();
        }

        public DomainResult Activate()
        {
            if (Status == ProductStatus.Deleted) return DomainResult.Failure(ProductErrors.AlreadyDeleted);
            Status = ProductStatus.Active;
            if (PublishedAt == null) PublishedAt = Clock.Now;
            return DomainResult.Success();
        }

        public DomainResult Delete()
        {
            if (Status == ProductStatus.Deleted) return DomainResult.Success();

            Status = ProductStatus.Deleted;

            var types = _productTypes.ToList();
            foreach (var type in types)
            {
                type.RemoveProduct(this);
            }
            _productTypes.Clear();

            return DomainResult.Success();
        }

        public DomainResult Deactivate()
        {
            if (Status == ProductStatus.Deleted) return DomainResult.Failure(ProductErrors.AlreadyDeleted);
            Status = ProductStatus.Inactive;
            return DomainResult.Success();
        }

        public void SetDraft() => Status = ProductStatus.Draft;

        protected void LoadState(
            Guid id,
            string title,
            string author,
            long price,
            ProductStatus status,
            DateTime? publishedAt,
            BlobMetadata? blobMetadata,
            DateTime createdAt,
            string? createdBy,
            DateTime? updatedAt,
            string? updatedBy,
            bool isSoftDeleted,
            DateTime? deletedAt,
            string? deletedBy)
        {
            Id = id;
            Title = title;
            Author = author;
            Price = price;
            Status = status;
            PublishedAt = publishedAt;
            AdditionalInfo = null;

            if (blobMetadata is not null)
            {
                SetBlob(blobMetadata);
            }

            SetCreated(createdAt, createdBy);

            if (updatedAt.HasValue)
            {
                SetUpdated(updatedAt.Value, updatedBy);
            }

            if (isSoftDeleted && deletedAt.HasValue)
            {
                SetDeleted(deletedAt.Value, deletedBy);
            }
        }

        public void LoadProductTypes(IEnumerable<ProductType> productTypes)
        {
            _productTypes.Clear();

            foreach (var productType in productTypes)
            {
                AddProductType(productType);
            }
        }

        public void LoadMediaFiles(IEnumerable<ProductMedia> mediaFiles)
        {
            _mediaFiles.Clear();

            foreach (var media in mediaFiles)
            {
                if (_mediaFiles.Any(x => x.Id == media.Id))
                    continue;

                media.AttachProduct(this);
                _mediaFiles.Add(media);
            }
        }

        public void LoadCoverImage(ProductCoverImage? coverImage)
        {
            CoverImage = coverImage;

            if (CoverImage is not null)
            {
                CoverImage.AttachProduct(this);
            }
        }
    }
}
