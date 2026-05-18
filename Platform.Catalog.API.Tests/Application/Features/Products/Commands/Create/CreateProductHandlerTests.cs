using Microsoft.AspNetCore.Http;
using Platform.Catalog.API.Application.Abstractions.Stores;
using Platform.Catalog.API.Application.Features.Products.Commands.Create;
using Platform.Catalog.API.Domain.Enums;
using Platform.Catalog.API.Infrastructure.Persistence.Models;
using Platform.Catalog.API.Tests.Testing;
using Xunit;

namespace Platform.Catalog.API.Tests.Application.Features.Products.Commands.Create;

public sealed class CreateProductHandlerTests
{
    [Fact]
    public async Task Handle_WhenCurrentUserIsInvalid_Returns401()
    {
        var unitOfWork = new FakeUnitOfWork();
        unitOfWork.RegisterRepository(new FakeRepository<CategoryModel>());
        unitOfWork.RegisterRepository(new FakeRepository<ProductModel>());
        var handler = new CreateProductHandler(
            unitOfWork,
            new FakeCurrentUserProvider { CurrentUserId = "not-a-guid" },
            new FakeBlobService(),
            new FakeStorePolicyService());

        var result = await handler.Handle(new CreateProductCommand(CreateRequest(Guid.NewGuid())), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(StatusCodes.Status401Unauthorized, result.StatusCode);
    }

    [Fact]
    public async Task Handle_WhenStoreIsUnavailable_Returns400()
    {
        var category = CatalogFixtures.CreateCategoryModel();
        var categoryRepository = new FakeRepository<CategoryModel>(category);
        var productRepository = new FakeRepository<ProductModel>();
        var unitOfWork = new FakeUnitOfWork();
        unitOfWork.RegisterRepository(categoryRepository);
        unitOfWork.RegisterRepository(productRepository);
        var handler = new CreateProductHandler(
            unitOfWork,
            new FakeCurrentUserProvider { CurrentUserId = Guid.NewGuid().ToString() },
            new FakeBlobService(),
            new FakeStorePolicyService
            {
                CreateDecision = new CreateProductStorePolicyDecision
                {
                    Action = CreateProductStorePolicyAction.StoreUnavailable
                }
            });

        var result = await handler.Handle(new CreateProductCommand(CreateRequest(category.Id)), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        Assert.Equal(0, productRepository.AddCallCount);
    }

    [Fact]
    public async Task Handle_WhenRequestIsValid_AddsDraftProduct()
    {
        var category = CatalogFixtures.CreateCategoryModel(name: "Architecture");
        var categoryRepository = new FakeRepository<CategoryModel>(category);
        var productRepository = new FakeRepository<ProductModel>();
        var unitOfWork = new FakeUnitOfWork();
        unitOfWork.RegisterRepository(categoryRepository);
        unitOfWork.RegisterRepository(productRepository);
        var storeId = Guid.NewGuid();
        var handler = new CreateProductHandler(
            unitOfWork,
            new FakeCurrentUserProvider { CurrentUserId = Guid.NewGuid().ToString() },
            new FakeBlobService(),
            new FakeStorePolicyService
            {
                CreateDecision = new CreateProductStorePolicyDecision
                {
                    Action = CreateProductStorePolicyAction.Allowed,
                    StoreId = storeId
                }
            });

        var result = await handler.Handle(new CreateProductCommand(CreateRequest(category.Id)), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("Draft", result.Value.Status);
        Assert.Equal("Architecture", result.Value.CategoryName);
        Assert.Equal(1, productRepository.AddCallCount);
        var created = Assert.Single(productRepository.Entities);
        Assert.Equal(storeId, created.StoreId);
        Assert.Equal(ProductStatus.Draft, created.Status);
    }

    private static CreateProductRequest CreateRequest(Guid categoryId)
    {
        return new CreateProductRequest
        {
            Title = "Clean Architecture",
            Author = "Robert C. Martin",
            Price = 120_000,
            CategoryId = categoryId,
            Stock = 5
        };
    }
}
