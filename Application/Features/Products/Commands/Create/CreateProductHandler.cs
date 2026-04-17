using Platform.Application.Abstractions.Data;
using Platform.Application.Abstractions.Storage;
using Platform.Application.Messaging;
using Platform.BuildingBlocks.Abstractions;
using Platform.BuildingBlocks.Responses;
using Platform.Catalog.API.Application.Features.Products.Shared;
using Platform.Catalog.API.Application.Mappers;
using Platform.Catalog.API.Domain.Entities;
using Platform.Catalog.API.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace Platform.Catalog.API.Application.Features.Products.Commands.Create;

public sealed class CreateProductHandler : ICommandHandler<CreateProductCommand, ProductResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBlobService _blobService;
    private readonly ICurrentUserProvider _currentUserProvider;

    public CreateProductHandler(IUnitOfWork unitOfWork, IBlobService blobService, ICurrentUserProvider currentUserProvider)
    {
        _unitOfWork = unitOfWork;
        _blobService = blobService;
        _currentUserProvider = currentUserProvider;
    }

    public async Task<Result<ProductResponse>> Handle(CreateProductCommand command, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(_currentUserProvider.CurrentUserId, out var currentUserId))
            return Result<ProductResponse>.Failure("Current user is invalid.");

        var requestedTypeIds = command.Request.ProductTypeIds.Distinct().ToList();

        var productTypeModels = await _unitOfWork
            .GetRepository<ProductTypeModel>()
            .GetQueryable()
            .Where(x => requestedTypeIds.Contains(x.Id))
            .ToListAsync(cancellationToken);

        if (productTypeModels.Count != requestedTypeIds.Count)
            return Result<ProductResponse>.Failure("One or more product types are invalid.");

        var productTypes = productTypeModels.Select(x => x.ToDomain()).ToList();

        var createResult = ProductFactory.Create(
            command.Request.Kind,
            command.Request.Title,
            command.BlobName,
            command.ContainerName,
            command.Request.Author,
            command.Request.Price,
            productTypes,
            command.Request.Stock);

        if (!createResult.IsSuccess)
            return Result<ProductResponse>.Failure("Create failure");

        var product = createResult.Value;
        product.SetDraft();
        var productModel = product.ToPersistence(productTypeModels);

        await _unitOfWork.GetRepository<ProductModel>().AddAsync(productModel, cancellationToken);

        var blob = product.GetBlob();
        string? coverImageUrl = product.CoverImageUrl;

        if (blob is not null)
            coverImageUrl = _blobService.GenerateReadSasUrl(blob.ContainerName,blob.BlobName);

        return Result<ProductResponse>.Success(product.ToResponse(coverImageUrl));
    }
}
