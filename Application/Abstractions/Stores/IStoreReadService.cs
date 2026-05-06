namespace Platform.Catalog.API.Application.Abstractions.Stores;

public interface IStoreReadService
{
    Task<Guid?> GetCurrentOwnerStoreIdAsync(Guid userId, CancellationToken cancellationToken);
    Task<Guid?> GetStoreIdBySlugAsync(string slug, CancellationToken cancellationToken);
}
