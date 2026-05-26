# Platform.Catalog.API

## Public

- [x] `GET /api/catalog/products` - `Platform.MerchantUI/src/features/product/queries/product-queries.ts`
- [x] `GET /api/catalog/products/{productId}` - `Platform.MerchantUI/src/features/product/queries/product-queries.ts`
- [x] `GET /api/catalog/categories` - `Platform.MerchantUI/src/shared/lib/category-queries.ts`
- [x] `GET /api/catalog/stores/{slug}/products` - `Platform.MerchantUI/src/features/seller/queries/seller-queries.ts`
- [ ] `GET /api/catalog/product-medias`

## Authenticated

- [x] `GET /api/catalog/manage/products/me/pending` - `Platform.MerchantUI/src/features/store/queries/store-product-manage-queries.ts`
- [x] `POST /api/catalog/manage/products` - `Platform.MerchantUI/src/features/store/queries/store-product-manage-queries.ts`
- [x] `PUT /api/catalog/manage/products/{id}` - `Platform.MerchantUI/src/features/store/queries/store-product-manage-queries.ts`
- [x] `DELETE /api/catalog/manage/products/{id}` - `Platform.MerchantUI/src/features/store/queries/store-product-manage-queries.ts`
- [x] `POST /api/catalog/manage/products/{id}/approvals/owner` - `Platform.MerchantUI/src/features/store/queries/store-product-manage-queries.ts`
- [x] `GET /api/catalog/manage/stores/me/products/pending-owner-review` - `Platform.MerchantUI/src/features/store/queries/store-product-manage-queries.ts`

## Admin

- [ ] `POST /api/catalog/manage/categories`
- [ ] `PUT /api/catalog/manage/categories/{id}`
- [ ] `DELETE /api/catalog/manage/categories/{id}`
