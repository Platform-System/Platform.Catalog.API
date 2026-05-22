using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Platform.BuildingBlocks.Requests;
using Platform.BuildingBlocks.Responses;
using Platform.Catalog.API.Application.Features.Products.Queries.GetAll;
using Platform.Catalog.API.Application.Features.Products.Queries.GetById;

namespace Platform.Catalog.API.Presentation.Http;

[Route("api/products")]
[ApiController]
[Authorize]
public sealed class ProductsController : ControllerBase
{
    private readonly ISender _sender;

    public ProductsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Gets public products with paging and filters.
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll(
        [FromQuery] GetAllProductsRequest productsRequest,
        [FromQuery] PagingRequest pageRequest,
        CancellationToken cancellationToken)
    {
        var query = new GetAllProductsQuery(productsRequest)
        {
            Page = pageRequest.Page,
            PageSize = pageRequest.PageSize
        };

        var result = await _sender.Send(query, cancellationToken);
        return result.ToActionResult();
    }

    /// <summary>
    /// Gets a public product by id.
    /// </summary>
    [HttpGet("{productId:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(Guid productId, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetProductByIdQuery(productId), cancellationToken);
        return result.ToActionResult();
    }

}
