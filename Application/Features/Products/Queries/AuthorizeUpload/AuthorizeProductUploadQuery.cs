using Platform.Application.Messaging;
using Platform.Catalog.API.Application.Features.Products.Responses;

namespace Platform.Catalog.API.Application.Features.Products.Queries.AuthorizeUpload;

public sealed record AuthorizeProductUploadQuery(Guid ProductId) : IQuery<ProductUploadAuthorizationResponse>;
