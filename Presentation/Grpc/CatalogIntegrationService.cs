using Grpc.Core;
using Platform.Application.Abstractions.Data;
using Platform.Catalog.API.Domain.Enums;
using Platform.Catalog.API.Infrastructure.Persistence.Models;
using Platform.Catalog.Grpc;

namespace Platform.Catalog.API.Presentation.Grpc;

public sealed class CatalogIntegrationService : CatalogIntegration.CatalogIntegrationBase
{
    private readonly IUnitOfWork _unitOfWork;

    public CatalogIntegrationService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public override async Task<GetProductCartSnapshotResponse> GetProductCartSnapshot(
        GetProductCartSnapshotRequest request,
        ServerCallContext context)
    {
        // Catalog sở hữu dữ liệu product, nên service khác chỉ đọc qua
        // endpoint gRPC này thay vì query trực tiếp CatalogDb.
        if (!Guid.TryParse(request.ProductId, out var productId))
            return CatalogIntegrationResponses.Failure(CatalogErrorCodeGrpc.InvalidProductId, "Invalid product id.");

        var product = await _unitOfWork.GetRepository<ProductModel>().FindAsync(
            x => x.Id == productId && x.Status == ProductStatus.Active,
            true,
            context.CancellationToken);

        if (product is null)
            return CatalogIntegrationResponses.Failure(CatalogErrorCodeGrpc.ProductNotFound, "Product not found.");

        return product.ToSuccessResponse();
    }
}
