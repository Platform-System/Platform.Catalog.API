using MediatR;
using Platform.Application.Messaging;

namespace Platform.Catalog.API.Application.Features.Categories.Commands.Delete;

public sealed class DeleteCategoryCommand : ICommand
{
    public DeleteCategoryCommand(Guid categoryId)
    {
        CategoryId = categoryId;
    }

    public Guid CategoryId { get; }
}
