using Platform.Catalog.API.Domain.Enums;
using Platform.Catalog.API.Domain.ValueObjects;

namespace Platform.Catalog.API.Domain.Entities;

public sealed record ProductLoadData(
    Guid Id,
    string Title,
    string Author,
    long Price,
    ProductStatus Status,
    DateTime? PublishedAt,
    BlobMetadata? BlobMetadata,
    DateTime CreatedAt,
    string? CreatedBy,
    DateTime? UpdatedAt,
    string? UpdatedBy,
    bool IsSoftDeleted,
    DateTime? DeletedAt,
    string? DeletedBy);
