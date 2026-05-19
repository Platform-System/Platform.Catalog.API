using Microsoft.AspNetCore.Http;
using Platform.Catalog.API.Application.Features.Products.Mappers;
using Platform.Catalog.API.Application.Features.Products.Services;
using Platform.Catalog.API.Domain.Enums;
using Platform.Catalog.API.Infrastructure.Persistence.Models;
using Platform.Catalog.API.Tests.Testing;
using Xunit;

namespace Platform.Catalog.API.Tests.Application.Features.Products.Services;

public sealed class ProductApprovalServiceTests
{
    [Fact]
    public async Task PublishActiveAsync_WhenBlobIsMissing_Returns404()
    {
        var category = CatalogFixtures.CreateCategoryModel();
        var productModel = CatalogFixtures.CreateProductModel(category, status: ProductStatus.PendingOwnerReview);
        var product = productModel.ToDomain();
        var repository = new FakeRepository<ProductModel>(productModel);
        var unitOfWork = new FakeUnitOfWork();
        unitOfWork.RegisterRepository(repository);
        var service = new ProductApprovalService(unitOfWork, new FakeBlobService());

        var result = await service.PublishActiveAsync(productModel, product, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
        Assert.Equal(0, repository.UpdateCallCount);
    }

    [Fact]
    public async Task PublishActiveAsync_WhenBlobExists_PublishesBlobAndActivatesProduct()
    {
        var category = CatalogFixtures.CreateCategoryModel();
        var productModel = CatalogFixtures.CreateProductModel(
            category,
            status: ProductStatus.PendingOwnerReview,
            withBlob: true,
            withCoverImage: true);
        var product = productModel.ToDomain();
        var repository = new FakeRepository<ProductModel>(productModel);
        var unitOfWork = new FakeUnitOfWork();
        unitOfWork.RegisterRepository(repository);
        var blobService = new FakeBlobService();
        var service = new ProductApprovalService(unitOfWork, blobService);

        var result = await service.PublishActiveAsync(productModel, product, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(ProductStatus.Active, productModel.Status);
        Assert.NotNull(productModel.PublishedAt);
        Assert.Equal(2, blobService.MakePublicCallCount);
        Assert.Equal(1, repository.UpdateCallCount);
        Assert.Equal("https://blob.local/catalog-cover/cover.jpg", productModel.CoverImage?.Url);
    }

    [Fact]
    public async Task PublishActiveAsync_ForwardsCancellationTokenToBlobService()
    {
        var category = CatalogFixtures.CreateCategoryModel();
        var productModel = CatalogFixtures.CreateProductModel(
            category,
            status: ProductStatus.PendingOwnerReview,
            withBlob: true,
            withCoverImage: true);
        var product = productModel.ToDomain();
        var repository = new FakeRepository<ProductModel>(productModel);
        var unitOfWork = new FakeUnitOfWork();
        unitOfWork.RegisterRepository(repository);
        var blobService = new FakeBlobService();
        var service = new ProductApprovalService(unitOfWork, blobService);
        using var cancellationTokenSource = new CancellationTokenSource();

        await service.PublishActiveAsync(productModel, product, cancellationTokenSource.Token);

        Assert.Equal(2, blobService.MakePublicCancellationTokens.Count);
        Assert.All(blobService.MakePublicCancellationTokens, token => Assert.Equal(cancellationTokenSource.Token, token));
    }
}
