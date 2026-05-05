using Platform.Catalog.API.Domain.Entities;
using Platform.Catalog.API.Infrastructure.Persistence.Models;

namespace Platform.Catalog.API.Application.Features.Categories.Mappers;

public static class CategoryPersistenceMapper
{
    public static CategoryModel ToPersistence(this Category category)
    {
        return new CategoryModel(category.Id)
        {
            Name = category.Name,
            Status = category.Status
        };
    }

    public static Category ToDomain(this CategoryModel model)
        => Category.Load(model.Id, model.Name, model.Status);

    public static void ApplyDomainState(this CategoryModel model, Category category)
    {
        model.Name = category.Name;
        model.Status = category.Status;
    }
}
