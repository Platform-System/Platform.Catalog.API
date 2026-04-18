using Platform.BuildingBlocks.Extensions;
using Platform.Catalog.API.Domain.Entities;
using Platform.Catalog.API.Domain.ValueObjects;
using Platform.Catalog.API.Infrastructure.Persistence.Models;

namespace Platform.Catalog.API.Application.Mappers;

public static class PersistenceMapper
{
    public static ProductModel ToPersistence(this Product product, IEnumerable<ProductTypeModel> productTypes)
    {
        ProductModel model = product switch
        {
            PhysicalProduct physical => new PhysicalProductModel(product.Id, physical.Stock),
            DigitalProduct => new DigitalProductModel(product.Id),
            _ => throw new InvalidOperationException($"Unsupported product domain type: {product.GetType().Name}")
        };

        model.Title = product.Title;
        model.Author = product.Author;
        model.Price = product.Price;
        model.Status = product.Status;
        model.PublishedAt = product.PublishedAt;
        model.AdditionalInfo = product.AdditionalInfo;
        model.ProductTypes = productTypes.ToList();

        return model;
    }

    public static void ApplyDomainState(this ProductModel model, Product product, IEnumerable<ProductTypeModel>? productTypes = null)
    {
        model.Title = product.Title;
        model.Author = product.Author;
        model.Price = product.Price;
        model.Status = product.Status;
        model.PublishedAt = product.PublishedAt;
        model.AdditionalInfo = product.AdditionalInfo;

        if (model is PhysicalProductModel physicalModel && product is PhysicalProduct physicalProduct)
        {
            physicalModel.Stock = physicalProduct.Stock;
        }

        if (productTypes is not null)
        {
            model.ProductTypes.Clear();

            foreach (var productType in productTypes)
            {
                model.ProductTypes.Add(productType);
            }
        }
    }

    public static Product ToDomain(this ProductModel model)
    {
        var loadData = model.ToLoadData();

        Product domain = model switch
        {
            PhysicalProductModel physical => PhysicalProduct.Load(loadData, physical.Stock),
            DigitalProductModel => DigitalProduct.Load(loadData),
            _ => throw new InvalidOperationException($"Unsupported product model type: {model.GetType().Name}")
        };

        domain.LoadProductTypes(model.ProductTypes.Select(ToDomain));
        domain.LoadMediaFiles(model.MediaFiles.Select(ToDomain));
        domain.LoadCoverImage(model.CoverImage is null ? null : ToDomain(model.CoverImage));

        return domain;
    }

    public static ProductType ToDomain(this ProductTypeModel model)
        => ProductType.Load(model.Id, model.Name, model.Status);

    public static ProductMedia ToDomain(this ProductMediaModel model)
        => ProductMedia.Load(model.Id, model.FileName, model.Url, model.ContentType, model.Size, model.Type, model.ProductId, model.SortOrder, model.AltText);

    public static ProductCoverImage ToDomain(this ProductCoverImageModel model)
        => ProductCoverImage.Load(model.Id, model.ProductId, model.BlobName, model.ContainerName, model.Url);

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
