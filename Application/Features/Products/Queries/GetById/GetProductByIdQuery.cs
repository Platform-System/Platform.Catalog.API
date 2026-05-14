using Platform.Application.Messaging;
using Platform.Catalog.API.Application.Features.Products.Responses;

namespace Platform.Catalog.API.Application.Features.Products.Queries.GetById;

public sealed record GetProductByIdQuery(Guid ProductId) : IQuery<ProductResponse>;
