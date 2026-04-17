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
        model.CoverImageUrl = product.CoverImageUrl;
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
        model.CoverImageUrl = product.CoverImageUrl;
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
        var blobMetadata = model.AdditionalInfo.GetProperty<BlobMetadata>("blob");

        Product domain = model switch
        {
            PhysicalProductModel physical => PhysicalProduct.Load(
                physical.Id,
                physical.Title,
                physical.CoverImageUrl,
                physical.Author,
                physical.Price,
                physical.Status,
                physical.PublishedAt,
                blobMetadata,
                physical.Stock,
                physical.CreatedAt,
                physical.CreatedBy,
                physical.UpdatedAt,
                physical.UpdatedBy,
                physical.IsSoftDeleted,
                physical.DeletedAt,
                physical.DeletedBy),
            DigitalProductModel digital => DigitalProduct.Load(
                digital.Id,
                digital.Title,
                digital.CoverImageUrl,
                digital.Author,
                digital.Price,
                digital.Status,
                digital.PublishedAt,
                blobMetadata,
                digital.CreatedAt,
                digital.CreatedBy,
                digital.UpdatedAt,
                digital.UpdatedBy,
                digital.IsSoftDeleted,
                digital.DeletedAt,
                digital.DeletedBy),
            _ => throw new InvalidOperationException($"Unsupported product model type: {model.GetType().Name}")
        };

        domain.LoadProductTypes(model.ProductTypes.Select(ToDomain));
        domain.LoadMediaFiles(model.MediaFiles.Select(ToDomain));

        return domain;
    }

    public static ProductType ToDomain(this ProductTypeModel model)
        => ProductType.Load(model.Id, model.Name);

    public static ProductMedia ToDomain(this ProductMediaModel model)
        => ProductMedia.Load(model.Id, model.FileName, model.Url, model.ContentType, model.Size, model.Type, model.ProductId, model.SortOrder, model.AltText);
}
