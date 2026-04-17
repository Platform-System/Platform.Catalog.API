using Platform.Catalog.API.Domain.Entities;
using Platform.Catalog.API.Infrastructure.Persistence.Models;

namespace Platform.Catalog.API.Application.Features.ProductTypes.Shared;

public static class ProductTypeMapper
{
    public static ProductTypeResponse ToResponse(this ProductTypeModel productType)
    {
        return new ProductTypeResponse
        {
            Id = productType.Id,
            Name = productType.Name,
            Status = productType.Status
        };
    }

    public static ProductTypeModel ToPersistence(this ProductType productType)
    {
        return new ProductTypeModel(productType.Id)
        {
            Name = productType.Name,
            Status = productType.Status
        };
    }

    public static void ApplyDomainState(this ProductTypeModel model, ProductType productType)
    {
        model.Name = productType.Name;
        model.Status = productType.Status;
    }
}
