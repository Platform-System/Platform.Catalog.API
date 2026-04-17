using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Platform.BuildingBlocks.Requests;
using Platform.BuildingBlocks.Responses;
using Platform.Catalog.API.Application.Features.ProductTypes.Commands.Create;
using Platform.Catalog.API.Application.Features.ProductTypes.Commands.Delete;
using Platform.Catalog.API.Application.Features.ProductTypes.Commands.Update;
using Platform.Catalog.API.Application.Features.ProductTypes.Queries.GetAll;

namespace Platform.Catalog.API.Presentation;

[Route("api/[controller]")]
[ApiController]
public sealed class ProductTypesController : ControllerBase
{
    private readonly ISender _sender;

    public ProductTypesController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PagingRequest request, CancellationToken cancellationToken)
    {
        var query = new GetAllProductTypesQuery
        {
            Page = request.Page,
            PageSize = request.PageSize
        };

        var result = await _sender.Send(query, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CreateProductTypeRequest request, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new CreateProductTypeCommand(request), cancellationToken);
        return result.ToActionResult();
    }

    [HttpPut("{productTypeId:guid}")]
    [Authorize]
    public async Task<IActionResult> Update(Guid productTypeId, [FromBody] UpdateProductTypeRequest request, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new UpdateProductTypeCommand(productTypeId, request), cancellationToken);
        return result.ToActionResult();
    }

    [HttpDelete("{productTypeId:guid}")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid productTypeId, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new DeleteProductTypeCommand(productTypeId), cancellationToken);
        return result.ToActionResult();
    }
}
