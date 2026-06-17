using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Platform.BuildingBlocks.Requests;
using Platform.BuildingBlocks.Responses;
using Platform.Catalog.API.Application.Features.Products.Commands.ApproveByOwner;
using Platform.Catalog.API.Application.Features.Products.Commands.Create;
using Platform.Catalog.API.Application.Features.Products.Commands.Delete;
using Platform.Catalog.API.Application.Features.Products.Commands.SetCover;
using Platform.Catalog.API.Application.Features.Products.Commands.SetMedias;
using Platform.Catalog.API.Application.Features.Products.Commands.Update;
using Platform.Catalog.API.Application.Features.Products.Queries.AuthorizeUpload;
using Platform.Catalog.API.Application.Features.Products.Queries.GetCurrentUserPendingProducts;

namespace Platform.Catalog.API.Presentation.Http;

[Route("api/manage/products")]
[ApiController]
[Authorize]
public sealed class ManageProductsController : ControllerBase
{
    private readonly ISender _sender;

    public ManageProductsController(ISender sender)
    {
        _sender = sender;
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
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductRequest request, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new UpdateProductCommand(id, request), cancellationToken);
        return result.ToActionResult();
    }

    /// <summary>
    /// Deletes a product.
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new DeleteProductCommand(id), cancellationToken);
        return result.ToActionResult();
    }

    /// <summary>
    /// Approves a product as store owner.
    /// </summary>
    [HttpPost("{id:guid}/approvals/owner")]
    public async Task<IActionResult> ApproveByOwner(Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new ApproveProductByOwnerCommand(id), cancellationToken);
        return result.ToActionResult();
    }

    /// <summary>
    /// Authorizes product asset upload.
    /// </summary>
    [HttpGet("{id:guid}/upload-authorization")]
    public async Task<IActionResult> AuthorizeProductUpload(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new AuthorizeProductUploadQuery(id), cancellationToken);
        return result.ToActionResult();
    }

    /// <summary>
    /// Sets product cover metadata.
    /// </summary>
    [HttpPut("{id:guid}/cover")]
    public async Task<IActionResult> SetProductCover(
        Guid id,
        [FromBody] SetProductCoverRequest request,
        CancellationToken cancellationToken)
    {
        var command = new SetProductCoverCommand(id, request);
        var result = await _sender.Send(command, cancellationToken);
        return result.ToActionResult();
    }

    /// <summary>
    /// Sets product media metadata.
    /// </summary>
    [HttpPut("{id:guid}/medias")]
    public async Task<IActionResult> SetProductMedias(
        Guid id,
        [FromBody] SetProductMediasRequest request,
        CancellationToken cancellationToken)
    {
        var command = new SetProductMediasCommand(id, request);
        var result = await _sender.Send(command, cancellationToken);
        return result.ToActionResult();
    }
}
