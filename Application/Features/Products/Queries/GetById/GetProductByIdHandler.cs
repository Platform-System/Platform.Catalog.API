using Microsoft.EntityFrameworkCore;
using Platform.Application.Abstractions.Data;
using Platform.Application.Abstractions.Storage;
using Platform.Application.Messaging;
using Platform.BuildingBlocks.Responses;
using Platform.Catalog.API.Application.Features.Products.Shared;
using Platform.Catalog.API.Domain.Enums;
using Platform.Catalog.API.Infrastructure.Persistence.Models;

namespace Platform.Catalog.API.Application.Features.Products.Queries.GetById;

public sealed class GetProductByIdHandler : IQueryHandler<GetProductByIdQuery, ProductResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBlobService _blobService;

    public GetProductByIdHandler(IUnitOfWork unitOfWork, IBlobService blobService)
    {
        _unitOfWork = unitOfWork;
        _blobService = blobService;
    }

    public async Task<Result<ProductResponse>> Handle(GetProductByIdQuery query, CancellationToken cancellationToken)
    {
        var product = await _unitOfWork
            .GetRepository<ProductModel>()
            .GetQueryable()
            .AsNoTracking()
            .Include(x => x.Category)
            .Include(x => x.CoverImage)
            .FirstOrDefaultAsync(
                x => x.Id == query.ProductId && x.Status == ProductStatus.Active,
                cancellationToken);

        if (product is null)
        {
            return Result<ProductResponse>.Failure("Product not found.");
        }

        return Result<ProductResponse>.Success(product.ToResponse(product.ResolveCoverImageUrl(_blobService)));
    }
}
