using Platform.Catalog.Grpc;

namespace Platform.Catalog.API.Presentation.Grpc;

public static class CatalogIntegrationResponses
{
    public static GetProductCartSnapshotResponse Failure(CatalogErrorCodeGrpc errorCode, string errorMessage)
        => new()
        {
            IsSuccess = false,
            ErrorCode = errorCode,
            ErrorMessage = errorMessage
        };
}
