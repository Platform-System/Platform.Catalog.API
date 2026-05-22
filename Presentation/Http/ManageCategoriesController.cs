using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Platform.BuildingBlocks.Responses;
using Platform.Catalog.API.Application.Features.Categories.Commands.Create;
using Platform.Catalog.API.Application.Features.Categories.Commands.Delete;
using Platform.Catalog.API.Application.Features.Categories.Commands.Update;

namespace Platform.Catalog.API.Presentation.Http;

[Route("api/manage/categories")]
[ApiController]
[Authorize]
public sealed class ManageCategoriesController : ControllerBase
{
    private readonly ISender _sender;

    public ManageCategoriesController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Creates a new category.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCategoryRequest request, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new CreateCategoryCommand(request), cancellationToken);
        return result.ToActionResult();
    }

    /// <summary>
    /// Updates an existing category.
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCategoryRequest request, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new UpdateCategoryCommand(id, request), cancellationToken);
        return result.ToActionResult();
    }

    /// <summary>
    /// Deletes a category.
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new DeleteCategoryCommand(id), cancellationToken);
        return result.ToActionResult();
    }
}
