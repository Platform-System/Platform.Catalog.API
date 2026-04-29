using Platform.Application.Messaging;
using Platform.Catalog.API.Application.Features.Categories.Shared;

namespace Platform.Catalog.API.Application.Features.Categories.Commands.Update;

public sealed class UpdateCategoryCommand : ICommand<CategoryResponse>
{
    public UpdateCategoryCommand(Guid categoryId, UpdateCategoryRequest request)
    {
        CategoryId = categoryId;
        Request = request;
    }

    public Guid CategoryId { get; }
    public UpdateCategoryRequest Request { get; }
}
