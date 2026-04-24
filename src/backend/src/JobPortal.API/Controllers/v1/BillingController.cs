using JobPortal.API.Models;
using JobPortal.Application.Common.Models;
using JobPortal.Application.Features.Billing.BillingFeatures;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobPortal.API.Controllers.v1;

[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
[Authorize]
public sealed class BillingController(IMediator mediator) : ControllerBase
{
    [HttpGet("subscription")]
    public async Task<IActionResult> GetSubscription(CancellationToken ct)
    {
        var result = await mediator.Send(new GetSubscriptionQuery(), ct);
        return Ok(ApiResponse<SubscriptionDto?>.Ok(result, "Subscription retrieved."));
    }

    [HttpPost("subscription/change")]
    public async Task<IActionResult> ChangePlan([FromBody] ChangePlanCommand command, CancellationToken ct)
    {
        await mediator.Send(command, ct);
        return Ok(ApiResponse.Ok("Plan changed."));
    }

    [HttpPost("subscription/cancel")]
    public async Task<IActionResult> CancelSubscription([FromBody] CancelSubscriptionRequest request, CancellationToken ct)
    {
        await mediator.Send(new CancelSubscriptionCommand(request.Reason), ct);
        return Ok(ApiResponse.Ok("Subscription cancelled."));
    }

    [HttpGet("invoices")]
    public async Task<IActionResult> GetInvoices(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetInvoicesQuery(pageNumber, pageSize), ct);
        return Ok(ApiResponse<PagedList<InvoiceDto>>.Ok(result, "Invoices retrieved."));
    }

    [HttpGet("credits")]
    public async Task<IActionResult> GetCredits(CancellationToken ct)
    {
        var result = await mediator.Send(new GetCreditsQuery(), ct);
        return Ok(ApiResponse<List<JobCreditDto>>.Ok(result, "Credits retrieved."));
    }

    [HttpPost("subscription/reactivate")]
    public async Task<IActionResult> ReactivateSubscription(CancellationToken ct)
    {
        await mediator.Send(new ReactivateSubscriptionCommand(), ct);
        return Ok(ApiResponse.Ok("Subscription reactivated."));
    }

    [HttpPost("credits/purchase")]
    public async Task<IActionResult> PurchaseCredits([FromBody] PurchaseCreditsRequest request, CancellationToken ct)
    {
        await mediator.Send(new PurchaseCreditsCommand(request.Quantity), ct);
        return Ok(ApiResponse.Ok("Credits purchased."));
    }

    [HttpGet("transactions")]
    public async Task<IActionResult> GetTransactions(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetTransactionsQuery(pageNumber, pageSize), ct);
        return Ok(ApiResponse<PagedList<TransactionDto>>.Ok(result, "Transactions retrieved."));
    }
}

public sealed record CancelSubscriptionRequest(string? Reason);
public sealed record PurchaseCreditsRequest(int Quantity);
