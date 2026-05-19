using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Platform.BuildingBlocks.Requests;
using Platform.BuildingBlocks.Responses;
using Platform.Catalog.API.Application.Features.Products.Queries.GetPendingOwnerApprovalProducts;
using Platform.Catalog.API.Application.Features.Products.Queries.GetProductsByStoreSlug;

namespace Platform.Catalog.API.Presentation.Http;

[Route("api/stores")]
[ApiController]
[Authorize]
public sealed class StoreProductsController : ControllerBase
{
    private readonly ISender _sender;

    public StoreProductsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Gets products waiting for owner review.
    /// </summary>
    [HttpGet("/api/manage/stores/me/products/pending-owner-review")]
    public async Task<IActionResult> GetPendingOwnerApprovalProducts([FromQuery] PagingRequest request, CancellationToken cancellationToken)
    {
        var query = new GetPendingOwnerApprovalProductsQuery
        {
            Page = request.Page,
            PageSize = request.PageSize
        };

        var result = await _sender.Send(query, cancellationToken);
        return result.ToActionResult();
    }

    /// <summary>
    /// Gets public products by store slug.
    /// </summary>
    [HttpGet("{slug}/products")]
    [AllowAnonymous]
    public async Task<IActionResult> GetProductsByStoreSlug(string slug, [FromQuery] PagingRequest request, CancellationToken cancellationToken)
    {
        var query = new GetProductsByStoreSlugQuery(slug)
        {
            Page = request.Page,
            PageSize = request.PageSize
        };

        var result = await _sender.Send(query, cancellationToken);
        return result.ToActionResult();
    }
}
