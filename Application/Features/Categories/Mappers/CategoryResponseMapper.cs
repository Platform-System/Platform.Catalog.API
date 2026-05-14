using Platform.Catalog.API.Application.Features.Categories.Responses;
using Platform.Catalog.API.Domain.Entities;
using Platform.Catalog.API.Infrastructure.Persistence.Models;

namespace Platform.Catalog.API.Application.Features.Categories.Mappers;

public static class CategoryResponseMapper
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

    public static CategoryResponse ToResponse(this Category category)
    {
        return new CategoryResponse
        {
            Id = category.Id,
            Name = category.Name,
            Status = category.Status
        };
    }
}
