using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Platform.BuildingBlocks.Requests;
using Platform.BuildingBlocks.Responses;
using Platform.Catalog.API.Application.Features.Products.Queries.GetPendingOwnerApprovalProducts;

namespace Platform.Catalog.API.Presentation.Http;

[Route("api/manage/stores/me/products")]
[ApiController]
[Authorize]
public sealed class ManageStoreProductsController : ControllerBase
{
    private readonly ISender _sender;

    public ManageStoreProductsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Gets products waiting for owner review.
    /// </summary>
    [HttpGet("pending-owner-review")]
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
}
