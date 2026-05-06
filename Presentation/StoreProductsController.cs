using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Platform.BuildingBlocks.Requests;
using Platform.BuildingBlocks.Responses;
using Platform.Catalog.API.Application.Features.Products.Queries.GetPendingOwnerReview;
using Platform.Catalog.API.Application.Features.Products.Queries.GetProductsByStoreSlug;

namespace Platform.Catalog.API.Presentation;

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

    [HttpGet("current/pending-owner-review")]
    public async Task<IActionResult> GetPendingOwnerReview([FromQuery] PagingRequest request, CancellationToken cancellationToken)
    {
        var query = new GetPendingOwnerReviewQuery
        {
            Page = request.Page,
            PageSize = request.PageSize
        };

        var result = await _sender.Send(query, cancellationToken);
        return result.ToActionResult();
    }

    [HttpGet("{slug}/products")]
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
