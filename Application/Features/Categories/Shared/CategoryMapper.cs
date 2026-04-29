using Platform.Catalog.API.Domain.Entities;
using Platform.Catalog.API.Infrastructure.Persistence.Models;

namespace Platform.Catalog.API.Application.Features.Categories.Shared;

public static class CategoryMapper
{
    public static CategoryResponse ToResponse(this CategoryModel category)
    {
        return new CategoryResponse
        {
            Id = category.Id,
            Name = category.Name,
            Status = category.Status
        };
    }

    public static CategoryModel ToPersistence(this Category category)
    {
        return new CategoryModel(category.Id)
        {
            Name = category.Name,
            Status = category.Status
        };
    }

    public static void ApplyDomainState(this CategoryModel model, Category category)
    {
        model.Name = category.Name;
        model.Status = category.Status;
    }
}
