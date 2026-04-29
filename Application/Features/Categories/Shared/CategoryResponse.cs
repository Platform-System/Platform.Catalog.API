using Platform.Catalog.API.Domain.Enums;

namespace Platform.Catalog.API.Application.Features.Categories.Shared;

public sealed class CategoryResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public CategoryStatus Status { get; init; }
}
