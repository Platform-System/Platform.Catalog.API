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

    public static AuthorizeProductCoverUploadResponse FailureAuthorizeProductCoverUpload(string errorMessage)
        => new()
        {
            Status = ResponseStatusExtensions.Failure(errorMessage)
        };

    public static AuthorizeProductCoverUploadResponse SuccessAuthorizeProductCoverUpload(ProductCoverUploadVisibility visibility)
        => new()
        {
            Status = ResponseStatusExtensions.Success(),
            Data = new ProductCoverUploadAuthorizationData
            {
                Visibility = visibility
            }
        };

    public static SetProductCoverResponse FailureSetProductCover(string errorMessage)
        => new()
        {
            Status = ResponseStatusExtensions.Failure(errorMessage)
        };

    public static SetProductCoverResponse SuccessSetProductCover()
        => new()
        {
            Status = ResponseStatusExtensions.Success()
        };

    public static SetProductMediasResponse FailureSetProductMedias(string errorMessage)
        => new()
        {
            Status = ResponseStatusExtensions.Failure(errorMessage)
        };

    public static SetProductMediasResponse SuccessSetProductMedias()
        => new()
        {
            Status = ResponseStatusExtensions.Success()
        };
}
