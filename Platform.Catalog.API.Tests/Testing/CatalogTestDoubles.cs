using System.Linq.Expressions;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Storage;
using Platform.Application.Abstractions.Data;
using Platform.Application.Abstractions.Storage;
using Platform.BuildingBlocks.Abstractions;
using Platform.BuildingBlocks.Responses;
using Platform.BuildingBlocks.Json;
using Platform.Catalog.API.Application.Abstractions.Stores;
using Platform.Catalog.API.Domain.Entities;
using Platform.Catalog.API.Domain.Enums;
using Platform.Catalog.API.Domain.ValueObjects;
using Platform.Catalog.API.Infrastructure.Persistence.Models;
using Platform.Domain.Common;
using Platform.SystemContext.Abstractions;

namespace Platform.Catalog.API.Tests.Testing;

internal sealed class FakeUnitOfWork : IUnitOfWork
{
    private readonly Dictionary<Type, object> _repositories = [];

    public int SaveChangesCallCount { get; private set; }
    public bool HasActiveTransaction => false;

    public void RegisterRepository<T>(FakeRepository<T> repository) where T : Entity
    {
        _repositories[typeof(T)] = repository;
    }

    public IGenericRepository<T> GetRepository<T>() where T : Entity
    {
        if (_repositories.TryGetValue(typeof(T), out var repository))
        {
            return (IGenericRepository<T>)repository;
        }

        throw new NotSupportedException($"Repository for {typeof(T).Name} is not registered.");
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        SaveChangesCallCount++;
        return Task.FromResult(1);
    }

    public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }
}

internal sealed class FakeRepository<T> : IGenericRepository<T> where T : Entity
{
    private readonly List<T> _entities = [];

    public FakeRepository(params T[] entities)
    {
        _entities.AddRange(entities);
    }

    public int AddCallCount { get; private set; }
    public int UpdateCallCount { get; private set; }
    public IReadOnlyList<T> Entities => _entities;

    public Task<T?> FindAsync(Expression<Func<T, bool>> predicate, bool asNoTracking = true, CancellationToken cancellationToken = default, params Expression<Func<T, object>>[] includes)
    {
        var compiled = predicate.Compile();
        return Task.FromResult(_entities.FirstOrDefault(compiled));
    }

    public Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        AddCallCount++;
        _entities.Add(entity);
        return Task.CompletedTask;
    }

    public void Update(T entity)
    {
        UpdateCallCount++;
    }

    public Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default, params Expression<Func<T, object>>[] includes) => throw new NotSupportedException();
    public Task<List<T>> GetAllAsync(CancellationToken cancellationToken = default, params Expression<Func<T, object>>[] includes) => throw new NotSupportedException();
    public Task<List<T>> GetAllAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default, params Expression<Func<T, object>>[] includes) => throw new NotSupportedException();
    public Task<PagedResult<T>> GetPagedAsync(int page, int pageSize, Expression<Func<T, bool>>? filter = null, Expression<Func<T, object>>? orderBy = null, bool isDescending = false, CancellationToken cancellationToken = default, params Expression<Func<T, object>>[] includes) => throw new NotSupportedException();
    public void Remove(T entity) => throw new NotSupportedException();
    public Task<int> DeleteRangeAsync(Expression<Func<T, bool>> predicate) => throw new NotSupportedException();
    public Task<int> DeleteInBatchesAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, DateTime>> orderBy, Expression<Func<T, Guid>> keySelector, int batchSize = 100) => throw new NotSupportedException();
    public Task<int> TotalAsync(Expression<Func<T, bool>> predicate) => throw new NotSupportedException();
}

internal sealed class FakeCurrentUserProvider : ICurrentUserProvider
{
    public string? CurrentUserId { get; init; }
}

internal sealed class FakeUserContext : IUserContext
{
    public Guid? UserId { get; init; }
    public string? Email => null;
    public string? UserName => null;
    public IReadOnlyCollection<string> Roles { get; } = [];
    public bool IsAuthenticated => UserId.HasValue;
    public bool IsInRole(string role) => false;
}

internal sealed class FakeBlobService : IBlobService
{
    public int MakePublicCallCount { get; private set; }
    public List<CancellationToken> MakePublicCancellationTokens { get; } = [];

    public Task<string> UploadAsync(Stream fileStream, string fileName, string contentType, string containerName)
    {
        throw new NotSupportedException();
    }

    public Task<(string BlobName, string ContainerName)> UploadAsync(Stream stream, string fileName, string contentType, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    public string GenerateReadSasUrl(string container, string blobName, int expireMinutes = 5)
    {
        return $"https://blob.local/{container}/{blobName}?sas=1";
    }

    public List<string> GenerateReadSasUrlsAsync(string container, IEnumerable<string> blobNames, int expireMinutes = 5)
    {
        return blobNames.Select(blobName => GenerateReadSasUrl(container, blobName, expireMinutes)).ToList();
    }

    public Task<string> MakePublicAndGetUrl(string container, string blobName, CancellationToken cancellationToken = default)
    {
        MakePublicCallCount++;
        MakePublicCancellationTokens.Add(cancellationToken);
        return Task.FromResult($"https://blob.local/{container}/{blobName}");
    }
}

internal sealed class FakeStorePolicyService : IStorePolicyService
{
    public CreateProductStorePolicyDecision CreateDecision { get; init; } = new() { Action = CreateProductStorePolicyAction.Allowed };
    public ManageStoreProductPolicyDecision ManageDecision { get; init; } = new() { Action = ManageStoreProductPolicyAction.Allowed };
    public OwnerStoreApprovalPolicyDecision ApprovalDecision { get; init; } = new() { Action = OwnerStoreApprovalPolicyAction.NotReady };

    public Task<CreateProductStorePolicyDecision> ResolveCreateProductAsync(Guid userId, CancellationToken cancellationToken)
    {
        return Task.FromResult(CreateDecision);
    }

    public Task<ManageStoreProductPolicyDecision> ResolveManageProductAsync(Guid userId, Guid storeId, string? creatorUserId, CancellationToken cancellationToken)
    {
        return Task.FromResult(ManageDecision);
    }

    public Task<OwnerStoreApprovalPolicyDecision> ResolveOwnerStoreApprovalAsync(Guid userId, Guid storeId, string? creatorUserId, ProductStatus productStatus, CancellationToken cancellationToken)
    {
        return Task.FromResult(ApprovalDecision);
    }
}

internal static class CatalogFixtures
{
    public static CategoryModel CreateCategoryModel(Guid? id = null, string name = "Books", CategoryStatus status = CategoryStatus.Active)
    {
        return new CategoryModel(id ?? Guid.NewGuid())
        {
            Name = name,
            Status = status
        };
    }

    public static ProductModel CreateProductModel(
        CategoryModel category,
        Guid? id = null,
        Guid? storeId = null,
        ProductStatus status = ProductStatus.Draft,
        int stock = 10,
        bool withBlob = false,
        bool withCoverImage = false,
        string? createdBy = "creator-user")
    {
        var productId = id ?? Guid.NewGuid();
        var model = new ProductModel(productId)
        {
            Title = "Domain-Driven Design",
            Author = "Eric Evans",
            Price = 100_000,
            Stock = stock,
            CategoryId = category.Id,
            Category = category,
            StoreId = storeId ?? Guid.NewGuid(),
            Status = status,
            MediaFiles = []
        };

        model.SetCreated(DateTime.UtcNow, createdBy);

        if (withBlob)
        {
            model.AdditionalInfo = new Dictionary<string, object?>
            {
                ["blob"] = new BlobMetadata
                {
                    BlobName = "product.json",
                    ContainerName = "catalog",
                    Status = BlobStatus.Private,
                    UploadedAt = DateTime.UtcNow
                }
            }.ToJsonDocument();
        }

        if (withCoverImage)
        {
            model.CoverImage = new ProductCoverImageModel
            {
                Id = Guid.NewGuid(),
                ProductId = productId,
                BlobName = "cover.jpg",
                ContainerName = "catalog-cover",
                FileName = "cover.jpg",
                ContentType = "image/jpeg",
                Size = 2048
            };
        }

        return model;
    }
}
