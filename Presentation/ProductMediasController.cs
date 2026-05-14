using MediatR;
using Microsoft.AspNetCore.Mvc;
using Platform.BuildingBlocks.Requests;
using Platform.BuildingBlocks.Responses;
using Platform.Catalog.API.Application.Features.ProductMedias.Queries.GetAll;

namespace Platform.Catalog.API.Presentation;

[Route("api/product-medias")]
[ApiController]
public sealed class ProductMediasController : ControllerBase
{
    private readonly ISender _sender;

    public ProductMediasController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] GetAllProductMediasRequest request,
        [FromQuery] PagingRequest pageRequest,
        CancellationToken cancellationToken)
    {
        var query = new GetAllProductMediasQuery(request)
        {
            Page = pageRequest.Page,
            PageSize = pageRequest.PageSize
        };

        var result = await _sender.Send(query, cancellationToken);
        return result.ToActionResult();
    }
}
