using Platform.Application.Abstractions.Data;
using Platform.Application.Abstractions.Storage;
using Platform.Application.Messaging;
using Platform.BuildingBlocks.Abstractions;
using Microsoft.AspNetCore.Http;
using Platform.BuildingBlocks.Responses;
using Platform.Catalog.API.Application.Features.Categories.Mappers;
using Platform.Catalog.API.Application.Features.Products.Mappers;
using Platform.Catalog.API.Application.Features.Products.Shared;
using Platform.Catalog.API.Domain.Entities;
using Platform.Catalog.API.Domain.Enums;
using Platform.Catalog.API.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace Platform.Catalog.API.Application.Features.Products.Commands.Create;

public sealed class CreateProductHandler : ICommandHandler<CreateProductCommand, ProductResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IBlobService _blobService;

    public CreateProductHandler(IUnitOfWork unitOfWork, ICurrentUserProvider currentUserProvider, IBlobService blobService)
    {
        _unitOfWork = unitOfWork;
        _currentUserProvider = currentUserProvider;
        _blobService = blobService;
    }

    public async Task<Result<ProductResponse>> Handle(CreateProductCommand command, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(_currentUserProvider.CurrentUserId, out var currentUserId))
            return Result<ProductResponse>.Failure(StatusCodes.Status401Unauthorized, "Current user is invalid.");

        var categoryModel = await _unitOfWork
            .GetRepository<CategoryModel>()
            .FindAsync(x => x.Id == command.Request.CategoryId && x.Status == CategoryStatus.Active, true, cancellationToken);

        if (categoryModel is null)
            return Result<ProductResponse>.Failure(StatusCodes.Status400BadRequest, "Category is invalid.");

        var createResult = Product.Create(
            command.Request.Title,
            command.Request.Author,
            command.Request.Price,
            categoryModel.ToDomain(),
            command.Request.Stock);

        if (!createResult.IsSuccess)
            return Result<ProductResponse>.Failure(StatusCodes.Status400BadRequest, "Create failure");

        var product = createResult.Value;
        product.SetDraft();
        var productModel = product.ToPersistence(categoryModel);

        await _unitOfWork.GetRepository<ProductModel>().AddAsync(productModel, cancellationToken);
        return Result<ProductResponse>.Success(product.ToResponse(productModel.ResolveCoverImageUrl(_blobService)));
    }
}
