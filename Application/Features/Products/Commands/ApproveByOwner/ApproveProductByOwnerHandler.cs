using MediatR;
using Microsoft.AspNetCore.Http;
using Platform.Application.Abstractions.Data;
using Platform.Application.Messaging;
using Platform.BuildingBlocks.Responses;
using Platform.Catalog.API.Application.Abstractions.Stores;
using Platform.Catalog.API.Application.Features.Products.Mappers;
using Platform.Catalog.API.Application.Features.Products.Services;
using Platform.Catalog.API.Domain.Enums;
using Platform.Catalog.API.Infrastructure.Persistence.Models;
using Platform.SystemContext.Abstractions;

namespace Platform.Catalog.API.Application.Features.Products.Commands.ApproveByOwner;

public sealed class ApproveProductByOwnerHandler : ICommandHandler<ApproveProductByOwnerCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ProductApprovalService _approvalService;
    private readonly IStorePolicyService _storePolicyService;
    private readonly IUserContext _userContext;

    public ApproveProductByOwnerHandler(IUnitOfWork unitOfWork, ProductApprovalService approvalService, IStorePolicyService storePolicyService, IUserContext userContext)
    {
        _unitOfWork = unitOfWork;
        _approvalService = approvalService;
        _storePolicyService = storePolicyService;
        _userContext = userContext;
    }

    public async Task<Result<Unit>> Handle(ApproveProductByOwnerCommand command, CancellationToken cancellationToken)
    {
        if (_userContext.UserId is not Guid currentActorId)
            return Result<Unit>.Failure(StatusCodes.Status401Unauthorized, "Current user is invalid.");

        var productModel = await _unitOfWork
            .GetRepository<ProductModel>()
            .FindAsync(
                x => x.Id == command.ProductId,
                false,
                cancellationToken,
                x => x.Category,
                x => x.MediaFiles,
                x => x.CoverImage!);

        if (productModel is null || productModel.Status == ProductStatus.Deleted)
            return Result<Unit>.Failure(StatusCodes.Status404NotFound, "Product not found.");

        if (productModel.Status == ProductStatus.Active)
            return Result<Unit>.Failure(StatusCodes.Status400BadRequest, "Product already approved.");

        var product = productModel.ToDomain();
        var decision = await _storePolicyService.ResolveOwnerStoreApprovalAsync(
            currentActorId,
            productModel.StoreId,
            productModel.CreatedBy,
            productModel.Status,
            cancellationToken);

        switch (decision.Action)
        {
            case OwnerStoreApprovalPolicyAction.PublishActive:
                return await _approvalService.PublishActiveAsync(productModel, product, cancellationToken);
            case OwnerStoreApprovalPolicyAction.MovePendingOwnerReview:
                var pendingOwnerResult = product.MarkPendingOwnerReview();
                if (pendingOwnerResult.IsFailure)
                    return Result<Unit>.Failure(StatusCodes.Status400BadRequest, "Unable to submit product for owner review.");
                productModel.ApplyDomainState(product);
                _unitOfWork.GetRepository<ProductModel>().Update(productModel);
                return Result<Unit>.Success(Unit.Value);
            case OwnerStoreApprovalPolicyAction.MovePendingAdminReview:
                var pendingAdminResult = product.MarkPendingAdminReview();
                if (pendingAdminResult.IsFailure)
                    return Result<Unit>.Failure(StatusCodes.Status400BadRequest, "Unable to submit product for admin review.");
                productModel.ApplyDomainState(product);
                _unitOfWork.GetRepository<ProductModel>().Update(productModel);
                return Result<Unit>.Success(Unit.Value);
            case OwnerStoreApprovalPolicyAction.ForbiddenStoreMembership:
                return Result<Unit>.Failure(StatusCodes.Status403Forbidden, "Current user does not belong to this store.");
            case OwnerStoreApprovalPolicyAction.ForbiddenCreatorOnly:
                return Result<Unit>.Failure(StatusCodes.Status403Forbidden, "Only the product creator can submit this product.");
            case OwnerStoreApprovalPolicyAction.ForbiddenOwnerOnlyUnverified:
                return Result<Unit>.Failure(StatusCodes.Status403Forbidden, "Only the store owner can submit products of unverified stores.");
            case OwnerStoreApprovalPolicyAction.ForbiddenOwnerOnlyApprove:
                return Result<Unit>.Failure(StatusCodes.Status403Forbidden, "Only the store owner can approve this product.");
            case OwnerStoreApprovalPolicyAction.UseAdminApproval:
                return Result<Unit>.Failure(StatusCodes.Status400BadRequest, "Use the admin approval API for this product.");
            case OwnerStoreApprovalPolicyAction.CreatorInvalid:
                return Result<Unit>.Failure(StatusCodes.Status400BadRequest, "Product creator is invalid.");
            default:
                return Result<Unit>.Failure(StatusCodes.Status400BadRequest, "Product is not ready for store owner approval.");
        }
    }
}
