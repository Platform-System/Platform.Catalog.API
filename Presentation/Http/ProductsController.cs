using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Platform.BuildingBlocks.Requests;
using Platform.BuildingBlocks.Responses;
using Platform.Catalog.API.Application.Features.Products.Commands.ApproveByOwner;
using Platform.Catalog.API.Application.Features.Products.Commands.Create;
using Platform.Catalog.API.Application.Features.Products.Commands.Delete;
using Platform.Catalog.API.Application.Features.Products.Commands.Update;
using Platform.Catalog.API.Application.Features.Products.Queries.GetAll;
using Platform.Catalog.API.Application.Features.Products.Queries.GetById;
using Platform.Catalog.API.Application.Features.Products.Queries.GetCurrentUserPendingProducts;

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

    /// <summary>
    /// Gets pending products for the current user.
    /// </summary>
    [HttpGet("me/pending")]
    public async Task<IActionResult> GetCurrentUserPendingProducts([FromQuery] PagingRequest request, CancellationToken cancellationToken)
    {
        var query = new GetCurrentUserPendingProductsQuery
        {
            Page = request.Page,
            PageSize = request.PageSize
        };

        var result = await _sender.Send(query, cancellationToken);
        return result.ToActionResult();
    }

    /// <summary>
    /// Creates a new product.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductRequest request, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new CreateProductCommand(request), cancellationToken);
        return result.ToActionResult();
    }

    /// <summary>
    /// Updates an existing product.
    /// </summary>
    [HttpPut("{productId:guid}")]
    public async Task<IActionResult> Update(Guid productId, [FromBody] UpdateProductRequest request, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new UpdateProductCommand(productId, request), cancellationToken);
        return result.ToActionResult();
    }


    /// <summary>
    /// Deletes a product.
    /// </summary>
    [HttpDelete("{productId:guid}")]
    public async Task<IActionResult> Delete(Guid productId, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new DeleteProductCommand(productId), cancellationToken);
        return result.ToActionResult();
    }

    /// <summary>
    /// Approves a product as store owner.
    /// </summary>
    [HttpPost("{productId:guid}/approvals/owner")]
    public async Task<IActionResult> ApproveByOwner(Guid productId, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new ApproveProductByOwnerCommand(productId), cancellationToken);
        return result.ToActionResult();
    }

}
