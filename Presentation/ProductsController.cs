using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Platform.BuildingBlocks.Requests;
using Platform.BuildingBlocks.Responses;
using Platform.Catalog.API.Application.Features.Products.Commands.ApproveProduct;
using Platform.Catalog.API.Application.Features.Products.Commands.Create;
using Platform.Catalog.API.Application.Features.Products.Commands.Delete;
using Platform.Catalog.API.Application.Features.Products.Commands.Update;
using Platform.Catalog.API.Application.Features.Products.Queries.GetAll;
using Platform.Catalog.API.Application.Features.Products.Queries.GetById;
using Platform.Catalog.API.Application.Features.Products.Queries.PendingProduct;
using Platform.Catalog.API.Application.Features.Products.Queries.PendingProductOfUser;

namespace Platform.Catalog.API.Presentation;

[Route("api/[controller]")]
[ApiController]
public sealed class ProductsController : ControllerBase
{
    private readonly ISender _sender;

    public ProductsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
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

    [HttpGet("{productId:guid}")]
    public async Task<IActionResult> GetById(Guid productId, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetProductByIdQuery(productId), cancellationToken);
        return result.ToActionResult();
    }

    [HttpGet("pending")]
    [Authorize]
    public async Task<IActionResult> GetAllPendingProducts([FromQuery] PagingRequest request, CancellationToken cancellationToken)
    {
        var query = new GetPendingProductQuery
        {
            Page = request.Page,
            PageSize = request.PageSize
        };

        var result = await _sender.Send(query, cancellationToken);
        return result.ToActionResult();
    }

    [HttpGet("users/me/pending")]
    [Authorize]
    public async Task<IActionResult> GetPendingProductsOfUser([FromQuery] PagingRequest request, CancellationToken cancellationToken)
    {
        var query = new GetPendingProductOfUserQuery
        {
            Page = request.Page,
            PageSize = request.PageSize
        };

        var result = await _sender.Send(query, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CreateProductRequest request, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new CreateProductCommand(request), cancellationToken);
        return result.ToActionResult();
    }

    [HttpPut("{productId:guid}")]
    [Authorize]
    public async Task<IActionResult> Update(Guid productId, [FromBody] UpdateProductRequest request, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new UpdateProductCommand(productId, request), cancellationToken);
        return result.ToActionResult();
    }


    [HttpDelete("{productId:guid}")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid productId, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new DeleteProductCommand(productId), cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost("{productId:guid}/approve")]
    [Authorize]
    public async Task<IActionResult> Approve(Guid productId, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new ApproveProductCommand(productId), cancellationToken);
        return result.ToActionResult();
    }
}
