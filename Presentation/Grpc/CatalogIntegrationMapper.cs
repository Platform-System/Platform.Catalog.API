using Platform.Catalog.API.Domain.Enums;
using Platform.Catalog.API.Infrastructure.Persistence.Models;
using Platform.Catalog.Grpc;
using Platform.Common.Grpc;

namespace Platform.Catalog.API.Presentation.Grpc;

public static class CatalogIntegrationMapper
{
    public static GetProductCartSnapshotResponse ToSuccessResponse(this ProductModel product)
    {
        // Chỉ trả các field Ordering cần để kiểm tra rule của cart.
        return new GetProductCartSnapshotResponse
        {
            Status = ResponseStatusExtensions.Success(),
            Data = new ProductCartSnapshotData
            {
                Id = product.Id.ToString(),
                Title = product.Title,
                Price = product.Price,
                IsActive = product.Status == ProductStatus.Active,
                Stock = product.Stock
            }
        };
    }

}
