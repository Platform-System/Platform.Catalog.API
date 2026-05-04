using Platform.Application.Messaging;
using Platform.Catalog.API.Application.Features.Products.Shared;

namespace Platform.Catalog.API.Application.Features.Products.Queries.GetById;

public sealed record GetProductByIdQuery(Guid ProductId) : IQuery<ProductResponse>;
