using MediatR;
using Microsoft.AspNetCore.Http;
using Platform.Application.Abstractions.Data;
using Platform.Application.Messaging;
using Platform.BuildingBlocks.Responses;
using Platform.Catalog.API.Application.Features.Products.Mappers;
using Platform.Catalog.API.Application.Features.Products.Services;
using Platform.Catalog.API.Domain.Enums;
using Platform.Catalog.API.Infrastructure.Persistence.Models;
using Platform.SystemContext.Abstractions;

namespace Platform.Catalog.API.Application.Features.Products.Commands.AdminApproveProduct;

public sealed class AdminApproveProductHandler : ICommandHandler<AdminApproveProductCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ProductApprovalService _approvalService;
    private readonly IUserContext _userContext;

    public AdminApproveProductHandler(IUnitOfWork unitOfWork, ProductApprovalService approvalService, IUserContext userContext)
    {
        _unitOfWork = unitOfWork;
        _approvalService = approvalService;
        _userContext = userContext;
    }

    public async Task<Result<Unit>> Handle(AdminApproveProductCommand command, CancellationToken cancellationToken)
    {
        if (!_userContext.IsInRole("admin"))
            return Result<Unit>.Failure(StatusCodes.Status403Forbidden, "Admin role is required to approve this product.");

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

        if (productModel.Status != ProductStatus.PendingAdminReview)
            return Result<Unit>.Failure(StatusCodes.Status400BadRequest, "Product is not waiting for admin approval.");

        return await _approvalService.PublishActiveAsync(productModel, productModel.ToDomain(), cancellationToken);
    }
}
