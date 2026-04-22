using Platform.Common.Grpc;
using Platform.Catalog.Grpc;

namespace Platform.Catalog.API.Presentation.Grpc;

public static class CatalogIntegrationResponses
{
    public static GetProductCartSnapshotResponse Failure(string errorMessage)
        => new()
        {
            Status = ResponseStatusExtensions.Failure(errorMessage)
        };

    public static AdjustStockResponse FailureAdjustStock(string errorMessage)
        => new()
        {
            Status = ResponseStatusExtensions.Failure(errorMessage)
        };
}
