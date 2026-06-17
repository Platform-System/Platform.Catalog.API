using Microsoft.AspNetCore.Http;
using Platform.Application.Abstractions.Data;
using Platform.Application.Messaging;
using Platform.BuildingBlocks.Responses;
using Platform.Catalog.API.Domain.Enums;
using Platform.Catalog.API.Infrastructure.Persistence.Models;
using Platform.SystemContext.Abstractions;

namespace Platform.Catalog.API.Application.Features.Products.Queries.AuthorizeUpload;

public sealed class AuthorizeProductUploadHandler : IQueryHandler<AuthorizeProductUploadQuery, ProductUploadAuthorizationResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;

    public AuthorizeProductUploadHandler(IUnitOfWork unitOfWork, IUserContext userContext)
    {
        _unitOfWork = unitOfWork;
        _userContext = userContext;
    }

    public async Task<Result<ProductUploadAuthorizationResponse>> Handle(AuthorizeProductUploadQuery query, CancellationToken cancellationToken)
    {
        if (_userContext.UserId is null)
            return Result<ProductUploadAuthorizationResponse>.Failure(StatusCodes.Status401Unauthorized, "Unauthorized.");

        var userId = _userContext.UserId.Value;

        var product = await _unitOfWork.GetRepository<ProductModel>().FindAsync(
            x => x.Id == query.ProductId && x.Status != ProductStatus.Deleted,
            true,
            cancellationToken);

        if (product is null)
            return Result<ProductUploadAuthorizationResponse>.Failure(StatusCodes.Status404NotFound, "Product not found.");

        if (product.Status == ProductStatus.Active)
            return Result<ProductUploadAuthorizationResponse>.Failure(StatusCodes.Status400BadRequest, "Active product assets cannot be updated.");

        if (!Guid.TryParse(product.CreatedBy, out var ownerId) || ownerId != userId)
            return Result<ProductUploadAuthorizationResponse>.Failure(StatusCodes.Status403Forbidden, "You do not own this product.");

        var isAdmin = _userContext.IsInRole("admin");
        var visibility = isAdmin ? "Public" : "Private";

        return Result<ProductUploadAuthorizationResponse>.Success(new ProductUploadAuthorizationResponse(visibility));
    }
}
