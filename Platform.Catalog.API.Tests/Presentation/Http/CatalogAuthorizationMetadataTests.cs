using Microsoft.AspNetCore.Authorization;
using Platform.Catalog.API.Presentation.Http;
using Xunit;

namespace Platform.Catalog.API.Tests.Presentation.Http;

public sealed class CatalogAuthorizationMetadataTests
{
    [Fact]
    public void Controllers_DefaultToAuthorize()
    {
        AssertHasAuthorizeOnClass(typeof(ProductsController));
        AssertHasAuthorizeOnClass(typeof(CategoriesController));
        AssertHasAuthorizeOnClass(typeof(ProductMediasController));
        AssertHasAuthorizeOnClass(typeof(StoreProductsController));
        AssertHasAuthorizeOnClass(typeof(ManageProductsController));
        AssertHasAuthorizeOnClass(typeof(ManageCategoriesController));
        AssertHasAuthorizeOnClass(typeof(ManageStoreProductsController));
    }

    [Fact]
    public void PublicReadActions_AllowAnonymous()
    {
        AssertHasAllowAnonymous(typeof(ProductsController), nameof(ProductsController.GetAll));
        AssertHasAllowAnonymous(typeof(ProductsController), nameof(ProductsController.GetById));
        AssertHasAllowAnonymous(typeof(CategoriesController), nameof(CategoriesController.GetAll));
        AssertHasAllowAnonymous(typeof(ProductMediasController), nameof(ProductMediasController.GetAll));
        AssertHasAllowAnonymous(typeof(StoreProductsController), nameof(StoreProductsController.GetProductsByStoreSlug));
    }

    private static void AssertHasAuthorizeOnClass(Type controllerType)
    {
        Assert.NotNull(controllerType.GetCustomAttributes(typeof(AuthorizeAttribute), inherit: true).SingleOrDefault());
    }

    private static void AssertHasAllowAnonymous(Type controllerType, string methodName)
    {
        var method = controllerType.GetMethod(methodName);
        Assert.NotNull(method);
        Assert.NotNull(method!.GetCustomAttributes(typeof(AllowAnonymousAttribute), inherit: true).SingleOrDefault());
    }
}
