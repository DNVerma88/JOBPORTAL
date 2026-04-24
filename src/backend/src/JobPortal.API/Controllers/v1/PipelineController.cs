using JobPortal.API.Models;
using JobPortal.Application.Features.Pipeline.PipelineFeatures;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobPortal.API.Controllers.v1;

[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
[Authorize]
public sealed class PipelineController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] Guid jobPostingId, CancellationToken ct)
    {
        var result = await mediator.Send(new GetPipelineQuery(jobPostingId), ct);
        return Ok(ApiResponse<PipelineResponse>.Ok(result, "Pipeline retrieved."));
    }

    [HttpPatch("candidates/{applicationId:guid}/stage")]
    public async Task<IActionResult> MoveStage(Guid applicationId, [FromBody] MoveStageRequest request, CancellationToken ct)
    {
        await mediator.Send(new MoveCandidateStageCommand(applicationId, request.StageId, request.Notes), ct);
        return Ok(ApiResponse.Ok("Candidate moved to stage."));
    }

    // ── Stage CRUD ────────────────────────────────────────────────────────────────

    [HttpPost("stages")]
    public async Task<IActionResult> CreateStage([FromBody] CreatePipelineStageCommand command, CancellationToken ct)
    {
        var id = await mediator.Send(command, ct);
        return Ok(ApiResponse<Guid>.Ok(id, "Pipeline stage created."));
    }

    [HttpPut("stages/{id:guid}")]
    public async Task<IActionResult> UpdateStage(Guid id, [FromBody] UpdateStageRequest request, CancellationToken ct)
    {
        await mediator.Send(new UpdatePipelineStageCommand(id, request.Name, request.Color, request.SortOrder), ct);
        return Ok(ApiResponse.Ok("Pipeline stage updated."));
    }

    [HttpDelete("stages/{id:guid}")]
    public async Task<IActionResult> DeleteStage(Guid id, CancellationToken ct)
    {
        await mediator.Send(new DeletePipelineStageCommand(id), ct);
        return Ok(ApiResponse.Ok("Pipeline stage deleted."));
    }
}

public sealed record MoveStageRequest(Guid StageId, string? Notes);
public sealed record UpdateStageRequest(string Name, string? Color, short SortOrder);
