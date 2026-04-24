using JobPortal.API.Models;
using JobPortal.Application.Common.Models;
using JobPortal.Application.Features.Jobs.Queries.GetJobs;
using JobPortal.Application.Features.SavedJobs.SavedJobFeatures;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobPortal.API.Controllers.v1;

[ApiController]
[Route("api/v1/saved-jobs")]
[Produces("application/json")]
[Authorize]
public sealed class SavedJobsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetSavedJobsQuery(pageNumber, pageSize), ct);
        return Ok(ApiResponse<PagedList<JobSummaryDto>>.Ok(result, "Saved jobs retrieved."));
    }

    [HttpPost]
    public async Task<IActionResult> Save([FromBody] SaveJobRequest request, CancellationToken ct)
    {
        var id = await mediator.Send(new SaveJobCommand(request.JobPostingId), ct);
        return Ok(ApiResponse<Guid>.Ok(id, "Job saved."));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Unsave(Guid id, CancellationToken ct)
    {
        await mediator.Send(new UnsaveJobCommand(id), ct);
        return Ok(ApiResponse.Ok("Job removed from saved list."));
    }
}

public sealed record SaveJobRequest(Guid JobPostingId);
