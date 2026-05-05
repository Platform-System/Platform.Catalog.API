using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Platform.BuildingBlocks.Responses;
using Platform.Catalog.API.Application.Features.StoreMembers.Commands.AcceptInvite;
using Platform.Catalog.API.Application.Features.StoreMembers.Commands.InviteMember;
using Platform.Catalog.API.Application.Features.Stores.Commands.ApproveVerification;
using Platform.Catalog.API.Application.Features.Stores.Commands.Create;
using Platform.Catalog.API.Application.Features.Stores.Commands.RequestVerification;
using Platform.Catalog.API.Application.Features.Stores.Queries.GetStoreByUser;

namespace Platform.Catalog.API.Presentation;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public sealed class StoresController : ControllerBase
{
    private readonly ISender _sender;

    public StoresController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("current")]
    public async Task<IActionResult> GetCurrentStore(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetStoreByUserQuery(), cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateStoreRequest request, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new CreateStoreCommand(request), cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost("{storeId:guid}/request-verification")]
    public async Task<IActionResult> RequestVerification(Guid storeId, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new RequestStoreVerificationCommand(storeId), cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost("{storeId:guid}/approve-verification")]
    public async Task<IActionResult> ApproveVerification(Guid storeId, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new ApproveStoreVerificationCommand(storeId), cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost("{storeId:guid}/invite-members")]
    public async Task<IActionResult> InviteMember(Guid storeId, [FromBody] InviteStoreMemberRequest request, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new InviteStoreMemberCommand(storeId, request), cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost("{storeId:guid}/accept-invite")]
    public async Task<IActionResult> AcceptInvite(Guid storeId, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new AcceptStoreInviteCommand(storeId), cancellationToken);
        return result.ToActionResult();
    }
}
