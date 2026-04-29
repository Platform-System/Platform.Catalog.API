using Platform.BuildingBlocks.DateTimes;
using Platform.Catalog.API.Domain.Enums;
using Platform.Catalog.API.Domain.Errors;
using Platform.Catalog.API.Domain.Extensions;
using Platform.Catalog.API.Domain.ValueObjects;
using Platform.Domain.Common;
using System.Text.Json;

namespace Platform.Catalog.API.Domain.Entities
{
    public class Product : AggregateRoot
    {
        public string Title { get; protected set; } = null!;
        public string Author { get; protected set; } = null!;
        public long Price { get; protected set; }
        public int Stock { get; private set; }
        public ProductStatus Status { get; protected set; }
        public DateTime? PublishedAt { get; protected set; }
        public JsonDocument? AdditionalInfo { get; protected set; }

        public Category Category { get; private set; } = null!;

        private readonly List<ProductMedia> _mediaFiles = new();
        public IReadOnlyCollection<ProductMedia> MediaFiles => _mediaFiles.AsReadOnly();
        public ProductCoverImage? CoverImage { get; private set; }

        protected Product() { }

        public static DomainResult<Product> Create(string title, string author, long price, Category category, int stock)
        {
            if (category is null)
                return DomainResult<Product>.Failure(ProductErrors.InvalidType);

            var product = new Product();
            var initializeResult = product.Initialize(title, author, price, category);
            if (initializeResult.IsFailure)
                return DomainResult<Product>.Failure(initializeResult.Error);

            var stockResult = product.SetStock(stock);
            if (stockResult.IsFailure)
                return DomainResult<Product>.Failure(stockResult.Error);

            return DomainResult<Product>.Success(product);
        }

        protected DomainResult Initialize(string title, string author, long price, Category category)
        {
            if (category is null)
                return DomainResult.Failure(ProductErrors.InvalidType);

            var infoResult = SetInfo(title, author, price);
            if (infoResult.IsFailure)
                return infoResult;

            SetCategory(category);

            return DomainResult.Success();
        }

        public DomainResult UpdateInfo(string title, string author, long price, Category category)
        {
            if (category is null)
                return DomainResult.Failure(ProductErrors.InvalidType);

            var validationResult = ValidateInfo(title, author, price);
            if (validationResult.IsFailure)
                return validationResult;

            Title = title.Trim();
            Author = author.Trim();
            Price = price;

            SetCategory(category);

            return DomainResult.Success();
        }

        public DomainResult UpdateStock(int quantity)
        {
            return SetStock(quantity);
        }

        private DomainResult SetStock(int quantity)
        {
            if (quantity < 0)
                return DomainResult.Failure(ProductErrors.InsufficientStock);

            Stock = quantity;
            return DomainResult.Success();
        }

        public DomainResult Restock(int quantity)
        {
            if (quantity <= 0)
                return DomainResult.Failure(DomainErrors.Validation.InvalidInput);

            Stock += quantity;
            return DomainResult.Success();
        }

        public DomainResult ReduceStock(int quantity)
        {
            if (quantity <= 0)
                return DomainResult.Failure(DomainErrors.Validation.InvalidInput);

            if (Stock < quantity)
                return DomainResult.Failure(ProductErrors.InsufficientStock);

            Stock -= quantity;
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

        public void SetCategory(Category category)
        {
            if (category == null) return;
            if (Category?.Id == category.Id) return;

            if (Category is not null)
            {
                Category.RemoveProduct(this);
            }

            Category = category;
            category.AddProduct(this);
        }

        public void RemoveCategory()
        {
            if (Category is null) return;

            Category.RemoveProduct(this);
            Category = null!;
        }

        public string GetCategoryName() => Category?.Name ?? string.Empty;

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

            return DomainResult.Success();
        }

        public DomainResult Deactivate()
        {
            if (Status == ProductStatus.Deleted) return DomainResult.Failure(ProductErrors.AlreadyDeleted);
            Status = ProductStatus.Inactive;
            return DomainResult.Success();
        }

        public void SetDraft() => Status = ProductStatus.Draft;

        public static Product Load(ProductLoadData loadData, int stock)
        {
            var product = new Product
            {
                Stock = stock
            };

            product.LoadState(
                loadData.Id,
                loadData.Title,
                loadData.Author,
                loadData.Price,
                loadData.Status,
                loadData.PublishedAt,
                loadData.BlobMetadata,
                loadData.CreatedAt,
                loadData.CreatedBy,
                loadData.UpdatedAt,
                loadData.UpdatedBy,
                loadData.IsSoftDeleted,
                loadData.DeletedAt,
                loadData.DeletedBy);

            return product;
        }

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

        public void LoadCategory(Category category)
        {
            SetCategory(category);
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

        public void SetCoverImage(ProductCoverImage coverImage)
        {
            coverImage.AttachProduct(this);
            CoverImage = coverImage;
        }

        public void SetMediaFiles(IEnumerable<ProductMedia> mediaFiles)
        {
            _mediaFiles.Clear();
            foreach (var media in mediaFiles)
            {
                media.AttachProduct(this);
                _mediaFiles.Add(media);
            }
        }
    }
}
