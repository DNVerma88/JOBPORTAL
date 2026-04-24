using JobPortal.API.Models;
using JobPortal.Application.Common.Models;
using JobPortal.Application.Features.JobAlerts.JobAlertFeatures;
using JobPortal.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobPortal.API.Controllers.v1;

[ApiController]
[Route("api/v1/job-alerts")]
[Produces("application/json")]
[Authorize]
public sealed class JobAlertsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetJobAlertsQuery(pageNumber, pageSize), ct);
        return Ok(ApiResponse<PagedList<JobAlertDto>>.Ok(result, "Job alerts retrieved."));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateJobAlertRequest request, CancellationToken ct)
    {
        var id = await mediator.Send(new CreateJobAlertCommand(
            request.Name, request.Frequency,
            request.Keyword, request.Location, request.JobType, request.WorkMode, request.ExperienceLevel), ct);
        return Ok(ApiResponse<Guid>.Ok(id, "Job alert created."));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateJobAlertRequest request, CancellationToken ct)
    {
        await mediator.Send(new UpdateJobAlertCommand(id, request.Frequency,
            request.Keyword, request.JobType, request.WorkMode, request.ExperienceLevel), ct);
        return Ok(ApiResponse.Ok("Job alert updated."));
    }

    [HttpPatch("{id:guid}/toggle")]
    public async Task<IActionResult> Toggle(Guid id, CancellationToken ct)
    {
        await mediator.Send(new ToggleJobAlertCommand(id), ct);
        return Ok(ApiResponse.Ok("Job alert toggled."));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await mediator.Send(new DeleteJobAlertCommand(id), ct);
        return Ok(ApiResponse.Ok("Job alert deleted."));
    }
}

public sealed record CreateJobAlertRequest(
    string? Name, AlertFrequency Frequency,
    string? Keyword = null, string? Location = null,
    string? JobType = null, string? WorkMode = null, string? ExperienceLevel = null);

public sealed record UpdateJobAlertRequest(
    AlertFrequency Frequency,
    string? Keyword = null, string? JobType = null,
    string? WorkMode = null, string? ExperienceLevel = null);
