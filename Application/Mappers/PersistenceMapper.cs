using Platform.BuildingBlocks.Extensions;
using Platform.Catalog.API.Domain.Entities;
using Platform.Catalog.API.Domain.ValueObjects;
using Platform.Catalog.API.Infrastructure.Persistence.Models;

namespace Platform.Catalog.API.Application.Mappers;

public static class PersistenceMapper
{
    // Ghi nhớ nhanh:
    // - ToDomain: ProductModel -> Product domain để xử lý nghiệp vụ
    // - ApplyDomainState: Product domain -> ProductModel cũ để cập nhật state rồi SaveChanges
    // - ToPersistence: Product domain -> ProductModel mới để Add vào database

    // Hàm này dùng khi cần tạo persistence model từ aggregate domain Product.
    // Mục đích là biến object domain đang xử lý trong business thành object EF model
    // để có thể Add vào DbContext và lưu xuống database.
    public static ProductModel ToPersistence(this Product product, CategoryModel category)
    {
        return new ProductModel(product.Id)
        {
            Title = product.Title,
            Author = product.Author,
            Price = product.Price,
            Stock = product.Stock,
            CategoryId = category.Id,
            Category = category,
            Status = product.Status,
            PublishedAt = product.PublishedAt,
            AdditionalInfo = product.AdditionalInfo
        };
    }

    // Hàm này dùng khi domain đã xử lý nghiệp vụ xong và mình muốn cập nhật ngược
    // state mới từ domain về persistence model đang được EF tracking.
    // Ví dụ: domain ReduceStock thành công thì hàm này sẽ copy Stock mới về model
    // để SaveChanges có thể lưu đúng dữ liệu xuống database.
    public static void ApplyDomainState(this ProductModel model, Product product, CategoryModel? category = null)
    {
        model.Title = product.Title;
        model.Author = product.Author;
        model.Price = product.Price;
        model.Stock = product.Stock;
        model.Status = product.Status;
        model.PublishedAt = product.PublishedAt;
        model.AdditionalInfo = product.AdditionalInfo;

        if (category is not null)
        {
            model.CategoryId = category.Id;
            model.Category = category;
        }
    }

    // Hàm này dùng để dựng aggregate domain Product từ dữ liệu persistence model.
    // Mục đích là đưa dữ liệu từ database về đúng domain object để tất cả business rule
    // như ReduceStock, Restock, Update... đều chạy qua domain thay vì sửa model trực tiếp.
    public static Product ToDomain(this ProductModel model)
    {
        var loadData = model.ToLoadData();
        var domain = Product.Load(loadData, model.Stock);

        // Nạp thêm các dữ liệu liên quan để aggregate domain có trạng thái đầy đủ.
        domain.LoadCategory(model.Category.ToDomain());
        domain.LoadMediaFiles(model.MediaFiles.Select(ToDomain));
        domain.LoadCoverImage(model.CoverImage is null ? null : ToDomain(model.CoverImage));

        return domain;
    }

    public static Category ToDomain(this CategoryModel model)
        => Category.Load(model.Id, model.Name, model.Status);

    public static ProductMedia ToDomain(this ProductMediaModel model)
        => ProductMedia.Load(model.Id, model.ProductId, model.BlobName, model.ContainerName, model.FileName, model.ContentType, model.Size, model.Type, model.SortOrder, model.AltText, model.Url);

    public static ProductCoverImage ToDomain(this ProductCoverImageModel model)
        => ProductCoverImage.Load(model.Id, model.ProductId, model.BlobName, model.ContainerName, model.FileName, model.ContentType, model.Size, model.AltText, model.Url);

    public static ProductCoverImageModel ToPersistence(this ProductCoverImage coverImage)
    {
        return new ProductCoverImageModel
        {
            Id = coverImage.Id,
            ProductId = coverImage.ProductId,
            BlobName = coverImage.BlobName,
            ContainerName = coverImage.ContainerName,
            FileName = coverImage.FileName,
            ContentType = coverImage.ContentType,
            Size = coverImage.Size,
            AltText = coverImage.AltText,
            Url = coverImage.Url
        };
    }

    public static ProductMediaModel ToPersistence(this ProductMedia media)
    {
        return new ProductMediaModel
        {
            Id = media.Id,
            ProductId = media.ProductId,
            BlobName = media.BlobName,
            ContainerName = media.ContainerName,
            FileName = media.FileName,
            ContentType = media.ContentType,
            Size = media.Size,
            AltText = media.AltText,
            Url = media.Url,
            Type = media.Type,
            SortOrder = media.SortOrder
        };
    }

    public static void ApplyDomainState(this ProductCoverImageModel model, ProductCoverImage coverImage)
    {
        model.BlobName = coverImage.BlobName;
        model.ContainerName = coverImage.ContainerName;
        model.FileName = coverImage.FileName;
        model.ContentType = coverImage.ContentType;
        model.Size = coverImage.Size;
        model.AltText = coverImage.AltText;
        model.Url = coverImage.Url;
    }

    public static void ApplyDomainState(this ProductMediaModel model, ProductMedia media)
    {
        model.BlobName = media.BlobName;
        model.ContainerName = media.ContainerName;
        model.FileName = media.FileName;
        model.ContentType = media.ContentType;
        model.Size = media.Size;
        model.AltText = media.AltText;
        model.Url = media.Url;
        model.Type = media.Type;
        model.SortOrder = media.SortOrder;
    }

    // Hàm phụ để gom dữ liệu chung của ProductModel thành ProductLoadData.
    // Sau đó ProductLoadData sẽ được truyền vào Product.Load
    // để khởi tạo lại aggregate domain từ dữ liệu đã lưu trong database.
    private static ProductLoadData ToLoadData(this ProductModel model)
    {
        return new ProductLoadData(
            model.Id,
            model.Title,
            model.Author,
            model.Price,
            model.Status,
            model.PublishedAt,
            model.AdditionalInfo.GetProperty<BlobMetadata>("blob"),
            model.CreatedAt,
            model.CreatedBy,
            model.UpdatedAt,
            model.UpdatedBy,
            model.IsSoftDeleted,
            model.DeletedAt,
            model.DeletedBy);
    }
}
