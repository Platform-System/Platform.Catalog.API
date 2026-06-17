using Platform.Application.Messaging;

namespace Platform.Catalog.API.Application.Features.Products.Queries.AuthorizeUpload;

public sealed record AuthorizeProductUploadQuery(Guid ProductId) : IQuery<ProductUploadAuthorizationResponse>;
