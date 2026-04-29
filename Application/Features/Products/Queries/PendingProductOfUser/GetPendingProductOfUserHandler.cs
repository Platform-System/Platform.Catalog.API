using Microsoft.EntityFrameworkCore;
using Platform.Application.Abstractions.Storage;
using Platform.Application.Abstractions.Data;
using Platform.Application.Messaging;
using Platform.BuildingBlocks.Abstractions;
using Platform.BuildingBlocks.Responses;
using Platform.Catalog.API.Application.Features.Products.Shared;
using Platform.Catalog.API.Domain.Enums;
using Platform.Catalog.API.Infrastructure.Persistence.Models;

namespace Platform.Catalog.API.Application.Features.Products.Queries.PendingProductOfUser;

public sealed class GetPendingProductOfUserHandler : IQueryHandler<GetPendingProductOfUserQuery, PagedResult<ProductResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IBlobService _blobService;

    public GetPendingProductOfUserHandler(IUnitOfWork unitOfWork, ICurrentUserProvider currentUserProvider, IBlobService blobService)
    {
        _unitOfWork = unitOfWork;
        _currentUserProvider = currentUserProvider;
        _blobService = blobService;
    }

    public async Task<Result<PagedResult<ProductResponse>>> Handle(GetPendingProductOfUserQuery query, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(_currentUserProvider.CurrentUserId, out var currentUserId))
            return Result<PagedResult<ProductResponse>>.Failure("Current user is invalid.");

        var userId = currentUserId.ToString();

        IQueryable<ProductModel> productQuery = _unitOfWork
            .GetRepository<ProductModel>()
            .GetQueryable()
            .AsNoTracking()
            .Include(x => x.Category)
            .Include(x => x.CoverImage)
            .Where(x => x.CreatedBy == userId && x.Status == ProductStatus.Draft);

        var totalCount = await productQuery.CountAsync(cancellationToken);

        if (totalCount == 0)
            return Result<PagedResult<ProductResponse>>.Failure("No pending products found for the user.");

        var productModels = await productQuery
            .OrderByDescending(x => x.CreatedAt)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);

        var items = productModels.Select(x => x.ToResponse(_blobService)).ToList();

        var pagedResult = new PagedResult<ProductResponse>
        {
            Items = items,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = totalCount
        };

        return Result<PagedResult<ProductResponse>>.Success(pagedResult);
    }
}
