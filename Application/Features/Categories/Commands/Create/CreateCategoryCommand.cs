using Platform.Application.Messaging;
using Platform.Catalog.API.Application.Features.Categories.Shared;

namespace Platform.Catalog.API.Application.Features.Categories.Commands.Create;

public sealed class CreateCategoryCommand : ICommand<CategoryResponse>
{
    public CreateCategoryCommand(CreateCategoryRequest request)
    {
        Request = request;
    }

    public CreateCategoryRequest Request { get; }
}
