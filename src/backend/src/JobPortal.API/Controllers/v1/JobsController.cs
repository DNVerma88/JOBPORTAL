using JobPortal.API.Models;
using JobPortal.Application.Common.Models;
using JobPortal.Application.Features.Applications.Queries.GetApplications;
using JobPortal.Application.Features.Jobs.Commands.CreateJob;
using JobPortal.Application.Features.Jobs.Commands.DeleteJob;
using JobPortal.Application.Features.Jobs.Commands.UpdateJob;
using JobPortal.Application.Features.Jobs.Commands.UpdateJobStatus;
using JobPortal.Application.Features.Jobs.Queries.GetJob;
using JobPortal.Application.Features.Jobs.Queries.GetJobs;
using JobPortal.Application.Features.SavedJobs.SavedJobFeatures;
using JobPortal.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobPortal.API.Controllers.v1;

[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
[Authorize]
public sealed class JobsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] string? status = null,
        [FromQuery] Guid? companyId = null,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetJobsQuery(pageNumber, pageSize, search, status, companyId), ct);
        return Ok(ApiResponse<PagedList<JobSummaryDto>>.Ok(result, "Jobs retrieved."));
    }

    /// <summary>Get all job postings created by the current user.</summary>
    [HttpGet("my")]
    public async Task<IActionResult> MyJobs(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] string? status = null,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetJobsQuery(pageNumber, pageSize, search, status, null, MineOnly: true), ct);
        return Ok(ApiResponse<PagedList<JobSummaryDto>>.Ok(result, "Your jobs retrieved."));
    }

    /// <summary>Get jobs saved by the current user (alias for /saved-jobs).</summary>
    [HttpGet("saved")]
    public async Task<IActionResult> GetSaved(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetSavedJobsQuery(pageNumber, pageSize), ct);
        return Ok(ApiResponse<PagedList<JobSummaryDto>>.Ok(result, "Saved jobs retrieved."));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new GetJobQuery(id), ct);
        if (result is null) return NotFound(ApiResponse.Fail("Job not found."));
        return Ok(ApiResponse<JobDetailDto>.Ok(result, "Job retrieved."));
    }

    /// <summary>Get applications for a specific job posting.</summary>
    [HttpGet("{jobId:guid}/applications")]
    public async Task<IActionResult> GetApplications(
        Guid jobId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? status = null,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetApplicationsQuery(pageNumber, pageSize, jobId, status, null), ct);
        return Ok(ApiResponse<PagedList<ApplicationSummaryDto>>.Ok(result, "Applications retrieved."));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateJobCommand command, CancellationToken ct)
    {
        var id = await mediator.Send(command, ct);
        return CreatedAtAction(nameof(Get), new { id }, ApiResponse<Guid>.Ok(id, "Job created."));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateJobCommand command, CancellationToken ct)
    {
        var updated = command with { Id = id };
        await mediator.Send(updated, ct);
        return Ok(ApiResponse.Ok("Job updated."));
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateJobStatusRequest request, CancellationToken ct)
    {
        await mediator.Send(new UpdateJobStatusCommand(id, request.Status), ct);
        return Ok(ApiResponse.Ok("Job status updated."));
    }

    /// <summary>Save a job posting for the current user.</summary>
    [HttpPost("{id:guid}/save")]
    public async Task<IActionResult> Save(Guid id, CancellationToken ct)
    {
        var savedId = await mediator.Send(new SaveJobCommand(id), ct);
        return Ok(ApiResponse<Guid>.Ok(savedId, "Job saved."));
    }

    /// <summary>Remove a job posting from the current user's saved list.</summary>
    [HttpDelete("{id:guid}/save")]
    public async Task<IActionResult> Unsave(Guid id, CancellationToken ct)
    {
        await mediator.Send(new UnsaveByJobPostingIdCommand(id), ct);
        return Ok(ApiResponse.Ok("Job removed from saved list."));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await mediator.Send(new DeleteJobCommand(id), ct);
        return Ok(ApiResponse.Ok("Job deleted."));
    }
}

public sealed record UpdateJobStatusRequest(JobPostingStatus Status);
