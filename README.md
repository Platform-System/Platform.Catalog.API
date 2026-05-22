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
- [ ] `GET /api/catalog/stores/{slug}/products`
- [ ] `GET /api/catalog/product-medias`

## Authenticated

- [ ] `GET /api/catalog/manage/products/me/pending`
- [ ] `POST /api/catalog/manage/products`
- [ ] `PUT /api/catalog/manage/products/{id}`
- [ ] `DELETE /api/catalog/manage/products/{id}`
- [ ] `POST /api/catalog/manage/products/{id}/approvals/owner`
- [ ] `GET /api/catalog/manage/stores/me/products/pending-owner-review`

## Admin

- [ ] `POST /api/catalog/manage/categories`
- [ ] `PUT /api/catalog/manage/categories/{id}`
- [ ] `DELETE /api/catalog/manage/categories/{id}`

## Notes

- `Platform.MerchantUI/src/shared/lib/category-queries.ts` van con fallback mock khi API loi, nhung endpoint category da duoc goi that.
- Co the mo rong README nay them cot `AdminUI`, `MerchantUI`, `PortalUI` neu ban muon tach theo consumer.
