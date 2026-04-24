using JobPortal.API.Models;
using JobPortal.Application.Common.Models;
using JobPortal.Application.Features.Offers.OfferFeatures;
using JobPortal.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobPortal.API.Controllers.v1;

[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
[Authorize]
public sealed class OffersController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] Guid? applicationId = null,
        [FromQuery] Guid? applicantId = null,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetOffersQuery(pageNumber, pageSize, applicationId, applicantId), ct);
        return Ok(ApiResponse<PagedList<OfferDto>>.Ok(result, "Offers retrieved."));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOfferCommand command, CancellationToken ct)
    {
        var id = await mediator.Send(command, ct);
        return Ok(ApiResponse<Guid>.Ok(id, "Offer created."));
    }

    /// <summary>Candidate accepts or declines an offer.</summary>
    [HttpPatch("{id:guid}/respond")]
    public async Task<IActionResult> Respond(Guid id, [FromBody] RespondToOfferRequest request, CancellationToken ct)
    {
        await mediator.Send(new RespondToOfferCommand(id, request.Response, request.Message), ct);
        return Ok(ApiResponse.Ok($"Offer {request.Response.ToString().ToLower()}."));
    }

    /// <summary>Recruiter revokes an offer.</summary>
    [HttpPatch("{id:guid}/revoke")]
    public async Task<IActionResult> Revoke(Guid id, [FromBody] RevokeOfferRequest request, CancellationToken ct)
    {
        await mediator.Send(new RevokeOfferCommand(id, request.Reason), ct);
        return Ok(ApiResponse.Ok("Offer revoked."));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new GetOfferByIdQuery(id), ct);
        if (result is null) return NotFound(ApiResponse.Fail("Offer not found."));
        return Ok(ApiResponse<OfferDto>.Ok(result, "Offer retrieved."));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateOfferCommand command, CancellationToken ct)
    {
        await mediator.Send(command with { Id = id }, ct);
        return Ok(ApiResponse.Ok("Offer updated."));
    }

    [HttpGet("{id:guid}/download")]
    public IActionResult Download(Guid id)
    {
        // Offer letter PDF generation is not yet implemented.
        return StatusCode(501, ApiResponse.Fail("Offer letter download is not yet implemented."));
    }
}

public sealed record RespondToOfferRequest(JobPortal.Domain.Enums.OfferLetterStatus Response, string? Message);
public sealed record RevokeOfferRequest(string? Reason);
