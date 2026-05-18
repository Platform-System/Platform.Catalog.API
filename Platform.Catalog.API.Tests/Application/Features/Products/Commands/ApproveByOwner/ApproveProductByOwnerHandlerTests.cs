using MediatR;
using Microsoft.AspNetCore.Http;
using Platform.Catalog.API.Application.Abstractions.Stores;
using Platform.Catalog.API.Application.Features.Products.Commands.ApproveByOwner;
using Platform.Catalog.API.Application.Features.Products.Services;
using Platform.Catalog.API.Domain.Enums;
using Platform.Catalog.API.Infrastructure.Persistence.Models;
using Platform.Catalog.API.Tests.Testing;
using Xunit;

namespace Platform.Catalog.API.Tests.Application.Features.Products.Commands.ApproveByOwner;

public sealed class ApproveProductByOwnerHandlerTests
{
    [Fact]
    public async Task Handle_WhenPolicyMovesToOwnerReview_UpdatesProductStatus()
    {
        var category = CatalogFixtures.CreateCategoryModel();
        var product = CatalogFixtures.CreateProductModel(category, status: ProductStatus.Draft);
        var productRepository = new FakeRepository<ProductModel>(product);
        var unitOfWork = new FakeUnitOfWork();
        unitOfWork.RegisterRepository(productRepository);
        var handler = new ApproveProductByOwnerHandler(
            unitOfWork,
            new ProductApprovalService(unitOfWork, new FakeBlobService()),
            new FakeStorePolicyService
            {
                ApprovalDecision = new OwnerStoreApprovalPolicyDecision
                {
                    Action = OwnerStoreApprovalPolicyAction.MovePendingOwnerReview
                }
            },
            new FakeUserContext { UserId = Guid.NewGuid() });

        var result = await handler.Handle(new ApproveProductByOwnerCommand(product.Id), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(ProductStatus.PendingOwnerReview, product.Status);
        Assert.Equal(1, productRepository.UpdateCallCount);
    }

    [Fact]
    public async Task Handle_WhenOnlyOwnerCanApprove_Returns403()
    {
        var category = CatalogFixtures.CreateCategoryModel();
        var product = CatalogFixtures.CreateProductModel(category, status: ProductStatus.PendingOwnerReview);
        var productRepository = new FakeRepository<ProductModel>(product);
        var unitOfWork = new FakeUnitOfWork();
        unitOfWork.RegisterRepository(productRepository);
        var handler = new ApproveProductByOwnerHandler(
            unitOfWork,
            new ProductApprovalService(unitOfWork, new FakeBlobService()),
            new FakeStorePolicyService
            {
                ApprovalDecision = new OwnerStoreApprovalPolicyDecision
                {
                    Action = OwnerStoreApprovalPolicyAction.ForbiddenOwnerOnlyApprove
                }
            },
            new FakeUserContext { UserId = Guid.NewGuid() });

        var result = await handler.Handle(new ApproveProductByOwnerCommand(product.Id), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(StatusCodes.Status403Forbidden, result.StatusCode);
        Assert.Equal(ProductStatus.PendingOwnerReview, product.Status);
        Assert.Equal(0, productRepository.UpdateCallCount);
    }
}
