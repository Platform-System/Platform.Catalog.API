# Platform.Catalog.API

## Muc dich

Theo doi nhanh endpoint nao da duoc frontend su dung qua gateway.

## Quy uoc

- `[x]` = da thay frontend dang goi
- `[ ]` = chua thay frontend nao trong repo hien tai goi toi

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

## Notes

- Product management flow hien duoc xem la phan he cua chu shop / merchant, khong thuoc `AdminUI`.
- `Platform.MerchantUI` hien da dung API that cho product/category/storefront; phan mock con lai chi nen la du lieu trinh bay tam thoi khi backend chua co field tuong ung.
- `Platform.MerchantUI` da co UI quan ly san pham ngay trong khu quan tri gian hang: tao, sua, xoa san pham pending va duyet owner review.
- Co the mo rong README nay them cot `AdminUI`, `MerchantUI`, `PortalUI` neu ban muon tach theo consumer.
