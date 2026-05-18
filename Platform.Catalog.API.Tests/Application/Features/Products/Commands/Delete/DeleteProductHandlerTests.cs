using Microsoft.AspNetCore.Http;
using Platform.Catalog.API.Application.Abstractions.Stores;
using Platform.Catalog.API.Application.Features.Products.Commands.Delete;
using Platform.Catalog.API.Domain.Enums;
using Platform.Catalog.API.Infrastructure.Persistence.Models;
using Platform.Catalog.API.Tests.Testing;
using Xunit;

namespace Platform.Catalog.API.Tests.Application.Features.Products.Commands.Delete;

public sealed class DeleteProductHandlerTests
{
    [Fact]
    public async Task Handle_WhenUserCannotManageProduct_Returns403()
    {
        var category = CatalogFixtures.CreateCategoryModel();
        var product = CatalogFixtures.CreateProductModel(category);
        var productRepository = new FakeRepository<ProductModel>(product);
        var unitOfWork = new FakeUnitOfWork();
        unitOfWork.RegisterRepository(productRepository);
        var handler = new DeleteProductHandler(
            unitOfWork,
            new FakeCurrentUserProvider { CurrentUserId = Guid.NewGuid().ToString() },
            new FakeBlobService(),
            new FakeStorePolicyService
            {
                ManageDecision = new ManageStoreProductPolicyDecision
                {
                    Action = ManageStoreProductPolicyAction.Forbidden
                }
            });

        var result = await handler.Handle(new DeleteProductCommand(product.Id), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(StatusCodes.Status403Forbidden, result.StatusCode);
        Assert.Equal(ProductStatus.Draft, product.Status);
        Assert.Equal(0, productRepository.UpdateCallCount);
    }

    [Fact]
    public async Task Handle_WhenUserCanManageProduct_MarksProductDeleted()
    {
        var category = CatalogFixtures.CreateCategoryModel();
        var product = CatalogFixtures.CreateProductModel(category, status: ProductStatus.Active);
        var productRepository = new FakeRepository<ProductModel>(product);
        var unitOfWork = new FakeUnitOfWork();
        unitOfWork.RegisterRepository(productRepository);
        var handler = new DeleteProductHandler(
            unitOfWork,
            new FakeCurrentUserProvider { CurrentUserId = Guid.NewGuid().ToString() },
            new FakeBlobService(),
            new FakeStorePolicyService
            {
                ManageDecision = new ManageStoreProductPolicyDecision
                {
                    Action = ManageStoreProductPolicyAction.Allowed
                }
            });

        var result = await handler.Handle(new DeleteProductCommand(product.Id), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("Deleted", result.Value.Status);
        Assert.Equal(ProductStatus.Deleted, product.Status);
        Assert.Equal(1, productRepository.UpdateCallCount);
    }
}
