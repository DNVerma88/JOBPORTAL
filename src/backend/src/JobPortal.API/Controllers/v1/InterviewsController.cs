using JobPortal.API.Models;
using JobPortal.Application.Common.Models;
using JobPortal.Application.Features.Interviews.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobPortal.API.Controllers.v1;

[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
[Authorize]
public sealed class InterviewsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] Guid? applicationId = null,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetInterviewsQuery(pageNumber, pageSize, applicationId), ct);
        return Ok(ApiResponse<PagedList<InterviewDto>>.Ok(result, "Interviews retrieved."));
    }

    [HttpPost]
    public async Task<IActionResult> Schedule([FromBody] ScheduleInterviewCommand command, CancellationToken ct)
    {
        var id = await mediator.Send(command, ct);
        return Ok(ApiResponse<Guid>.Ok(id, "Interview scheduled."));
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateInterviewStatusRequest request, CancellationToken ct)
    {
        await mediator.Send(new UpdateInterviewStatusCommand(id, request.IsCancelled, request.CancelledReason), ct);
        return Ok(ApiResponse.Ok("Interview status updated."));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Reschedule(Guid id, [FromBody] RescheduleInterviewRequest request, CancellationToken ct)
    {
        await mediator.Send(new RescheduleInterviewCommand(id, request.ScheduledAt, request.DurationMinutes,
            request.MeetingLink, request.Location), ct);
        return Ok(ApiResponse.Ok("Interview rescheduled."));
    }
}

public sealed record UpdateInterviewStatusRequest(bool IsCancelled, string? CancelledReason);
public sealed record RescheduleInterviewRequest(
    DateTimeOffset ScheduledAt, short DurationMinutes, string? MeetingLink, string? Location);
