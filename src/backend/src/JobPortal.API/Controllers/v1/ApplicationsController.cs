using JobPortal.API.Models;
using JobPortal.Application.Common.Interfaces;
using JobPortal.Application.Common.Models;
using JobPortal.Application.Features.Applications.Commands.ApplyForJob;
using JobPortal.Application.Features.Applications.Commands.WithdrawApplication;
using JobPortal.Application.Features.Applications.Queries.GetApplicationDetail;
using JobPortal.Application.Features.Applications.Queries.GetApplications;
using JobPortal.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobPortal.API.Controllers.v1;

[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
[Authorize]
public sealed class ApplicationsController(IMediator mediator, ICurrentUserService currentUser) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] Guid? jobPostingId = null,
        [FromQuery] string? status = null,
        [FromQuery] Guid? applicantId = null,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetApplicationsQuery(pageNumber, pageSize, jobPostingId, status, applicantId), ct);
        return Ok(ApiResponse<PagedList<ApplicationSummaryDto>>.Ok(result, "Applications retrieved."));
    }

    /// <summary>Get the current user's own applications.</summary>
    [HttpGet("my")]
    public async Task<IActionResult> MyApplications(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? status = null,
        CancellationToken ct = default)
    {
        var userId = currentUser.UserId;
        var result = await mediator.Send(new GetApplicationsQuery(pageNumber, pageSize, null, status, userId), ct);
        return Ok(ApiResponse<PagedList<ApplicationSummaryDto>>.Ok(result, "Your applications retrieved."));
    }

    /// <summary>Get a single application by ID.</summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetOne(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new GetApplicationDetailQuery(id), ct);
        if (result is null) return NotFound(ApiResponse.Fail("Application not found."));
        return Ok(ApiResponse<ApplicationDetailDto>.Ok(result, "Application retrieved."));
    }

    [HttpPost]
    public async Task<IActionResult> Apply([FromBody] ApplyForJobCommand command, CancellationToken ct)
    {
        var id = await mediator.Send(command, ct);
        return Ok(ApiResponse<Guid>.Ok(id, "Application submitted."));
    }

    [HttpPatch("{id:guid}/withdraw")]
    public async Task<IActionResult> Withdraw(Guid id, CancellationToken ct)
    {
        await mediator.Send(new WithdrawApplicationCommand(id), ct);
        return Ok(ApiResponse.Ok("Application withdrawn."));
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateStatusRequest request, CancellationToken ct)
    {
        await mediator.Send(new UpdateApplicationStatusCommand(id, request.Status), ct);
        return Ok(ApiResponse.Ok("Application status updated."));
    }
}

public sealed record UpdateStatusRequest(ApplicationStatus Status);
