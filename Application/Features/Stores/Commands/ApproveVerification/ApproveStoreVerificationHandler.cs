using MediatR;
using Platform.Application.Abstractions.Data;
using Platform.Application.Messaging;
using Platform.BuildingBlocks.Responses;
using Platform.Catalog.API.Application.Features.Stores.Mappers;
using Platform.Catalog.API.Infrastructure.Persistence.Models;
using Platform.SystemContext.Abstractions;

namespace Platform.Catalog.API.Application.Features.Stores.Commands.ApproveVerification;

public sealed class ApproveStoreVerificationHandler : ICommandHandler<ApproveStoreVerificationCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;

    public ApproveStoreVerificationHandler(IUnitOfWork unitOfWork, IUserContext userContext)
    {
        _unitOfWork = unitOfWork;
        _userContext = userContext;
    }

    public async Task<Result<Unit>> Handle(ApproveStoreVerificationCommand command, CancellationToken cancellationToken)
    {
        if (!_userContext.IsInRole("admin"))
            return Result<Unit>.Failure(StatusCodes.Status403Forbidden, "Admin role is required.");

        var storeModel = await _unitOfWork
            .GetRepository<StoreModel>()
            .FindAsync(x => x.Id == command.StoreId, false, cancellationToken);

        if (storeModel is null)
            return Result<Unit>.Failure(StatusCodes.Status404NotFound, "Store not found.");

        var store = storeModel.ToDomain();
        var approveResult = store.ApproveVerification();
        if (approveResult.IsFailure)
            return Result<Unit>.Failure(StatusCodes.Status400BadRequest, "Unable to approve store verification.");

        storeModel.ApplyDomainState(store);
        return Result<Unit>.Success(Unit.Value);
    }
}
