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
        public string? CoverImageUrl { get; protected set; }
        public string Author { get; protected set; } = null!;
        public long Price { get; protected set; }
        public Guid ProductTypeId { get; protected set; }
        public ProductStatus Status { get; protected set; }
        public DateTime? PublishedAt { get; protected set; }
        public JsonDocument? AdditionalInfo { get; private set; }
        public ProductType ProductType { get; protected set; } = null!;
        private readonly List<ProductMedia> _mediaFiles = new();
        public IReadOnlyCollection<ProductMedia> MediaFiles => _mediaFiles.AsReadOnly();
        
        protected Product() { }

        protected Product(string title, string blobName, string containerName, string author, long price, ProductType productType)
        {
            SetProductType(productType);
            SetInfo(title, blobName, containerName, author, price);
        }

        public DomainResult UpdateInfo(string title, string blobName, string containerName, string author, long price, ProductType newType)
        {
            if (newType == null)
                return DomainResult.Failure(ProductErrors.InvalidType);

            ChangeProductType(newType);
            return SetInfo(title, blobName, containerName, author, price);
        }

        protected DomainResult SetInfo(string title, string blobName, string containerName, string author, long price)
        {
            if (string.IsNullOrWhiteSpace(title)) return DomainResult.Failure(DomainErrors.Validation.Required(nameof(Title)));
            if (string.IsNullOrWhiteSpace(blobName)) return DomainResult.Failure(DomainErrors.Validation.Required("BlobName"));
            if (string.IsNullOrWhiteSpace(containerName)) return DomainResult.Failure(DomainErrors.Validation.Required("ContainerName"));
            if (string.IsNullOrWhiteSpace(author)) return DomainResult.Failure(DomainErrors.Validation.Required(nameof(Author)));
            if (price < 0) return DomainResult.Failure(ProductErrors.InvalidPrice);

            Title = title.Trim();
            SetBlob(new BlobMetadata
            {
                BlobName = blobName,
                ContainerName = containerName,
                UploadedAt = Clock.Now
            });
            Author = author.Trim();
            Price = price;

            return DomainResult.Success();
        }

        public void SetBlob(BlobMetadata blob)
        {
            AdditionalInfo = AdditionalInfo.Set(AdditionalInfoExtensions.BlobKey, blob);
        }
        
        public BlobMetadata? GetBlob()
        {
            return AdditionalInfo.Get<BlobMetadata>(AdditionalInfoExtensions.BlobKey);
        }

        public DomainResult PublishBlob(string publicUrl)
        {
            if (string.IsNullOrWhiteSpace(publicUrl)) 
                return DomainResult.Failure(DomainErrors.Validation.Required("PublicUrl"));

            var blob = GetBlob();
            if (blob == null) 
                return DomainResult.Failure(DomainErrors.Global.NotFound("Blob", Id));

            blob.Status = BlobStatus.Public;
            SetBlob(blob);
            CoverImageUrl = publicUrl;

            return DomainResult.Success();
        }

        public void ChangeProductType(ProductType newType)
        {
            if (newType == null) return;
            if (ProductTypeId == newType.Id) return;

            ProductType?.RemoveProduct(this);
            newType.AddProduct(this);

            ProductType = newType;
            ProductTypeId = newType.Id;
        }

        protected void SetProductType(ProductType productType)
        {
            if (productType == null) return;

            ProductType = productType;
            ProductTypeId = productType.Id;
            productType.AddProduct(this);
        }

        public string GetProductTypeName() => ProductType?.Name ?? "No Type";

        public DomainResult Activate()
        {
            if (Status == ProductStatus.Deleted) 
                return DomainResult.Failure(ProductErrors.AlreadyDeleted);
            
            Status = ProductStatus.Active;
            
            if (PublishedAt == null)
            {
                PublishedAt = Clock.Now;
            }

            return DomainResult.Success();
        }

        public DomainResult Delete()
        {
            if (Status == ProductStatus.Deleted) 
                return DomainResult.Success();

            Status = ProductStatus.Deleted;
            ProductType?.RemoveProduct(this);
            return DomainResult.Success();
        }

        public DomainResult Deactivate()
        {
            if (Status == ProductStatus.Deleted) 
                return DomainResult.Failure(ProductErrors.AlreadyDeleted);
            
            Status = ProductStatus.Inactive;
            return DomainResult.Success();
        }

        public void SetDraft() => Status = ProductStatus.Draft;
    }
}
