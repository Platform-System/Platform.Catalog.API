using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Platform.BuildingBlocks.Requests;
using Platform.BuildingBlocks.Responses;
using Platform.Catalog.API.Application.Features.Categories.Commands.Create;
using Platform.Catalog.API.Application.Features.Categories.Commands.Delete;
using Platform.Catalog.API.Application.Features.Categories.Commands.Update;
using Platform.Catalog.API.Application.Features.Categories.Queries.GetAll;

namespace Platform.Catalog.API.Presentation;

[Route("api/categories")]
[ApiController]
public sealed class CategoriesController : ControllerBase
{
    private readonly ISender _sender;

    public CategoriesController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PagingRequest request, CancellationToken cancellationToken)
    {
        var query = new GetAllCategoriesQuery
        {
            Page = request.Page,
            PageSize = request.PageSize
        };

        var result = await _sender.Send(query, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CreateCategoryRequest request, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new CreateCategoryCommand(request), cancellationToken);
        return result.ToActionResult();
    }

    [HttpPut("{categoryId:guid}")]
    [Authorize]
    public async Task<IActionResult> Update(Guid categoryId, [FromBody] UpdateCategoryRequest request, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new UpdateCategoryCommand(categoryId, request), cancellationToken);
        return result.ToActionResult();
    }

    [HttpDelete("{categoryId:guid}")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid categoryId, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new DeleteCategoryCommand(categoryId), cancellationToken);
        return result.ToActionResult();
    }
}
