using JobPortal.API.Models;
using JobPortal.Application.Features.Candidates.Commands.Educations;
using JobPortal.Application.Features.Candidates.Commands.UpdateMyProfile;
using JobPortal.Application.Features.Candidates.Commands.UploadResume;
using JobPortal.Application.Features.Candidates.Commands.WorkExperiences;
using JobPortal.Application.Features.Candidates.Queries.GetCandidateById;
using JobPortal.Application.Features.Candidates.Queries.GetCandidates;
using JobPortal.Application.Features.Candidates.Queries.GetMyProfile;
using JobPortal.Application.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobPortal.API.Controllers.v1;

[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
[Authorize]
public sealed class CandidatesController(IMediator mediator) : ControllerBase
{
    [HttpGet("me")]
    public async Task<IActionResult> GetMyProfile(CancellationToken ct)
    {
        var result = await mediator.Send(new GetMyProfileQuery(), ct);
        return Ok(ApiResponse<CandidateProfileDto?>.Ok(result, "Profile retrieved."));
    }

    [HttpPut("me")]
    public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateMyProfileCommand command, CancellationToken ct)
    {
        var id = await mediator.Send(command, ct);
        return Ok(ApiResponse<Guid>.Ok(id, "Profile updated."));
    }

    [HttpPost("me/resume")]
    [RequestSizeLimit(6_000_000)]
    public async Task<IActionResult> UploadResume(IFormFile file, CancellationToken ct)
    {
        if (file is null || file.Length == 0)
            return BadRequest(ApiResponse.Fail("No file uploaded."));

        using var stream = file.OpenReadStream();
        var url = await mediator.Send(new UploadResumeCommand(
            FileStream: stream,
            FileName: file.FileName,
            ContentType: file.ContentType,
            FileSize: file.Length), ct);

        return Ok(ApiResponse<string>.Ok(url, "Resume uploaded successfully."));
    }

    // ── Work Experiences ──────────────────────────────────────────────────────

    [HttpGet("me/work-experiences")]
    public async Task<IActionResult> GetWorkExperiences(CancellationToken ct)
    {
        var result = await mediator.Send(new GetWorkExperiencesQuery(), ct);
        return Ok(ApiResponse<List<WorkExperienceDto>>.Ok(result, "Work experiences retrieved."));
    }

    [HttpPost("me/work-experiences")]
    public async Task<IActionResult> AddWorkExperience([FromBody] AddWorkExperienceCommand command, CancellationToken ct)
    {
        var id = await mediator.Send(command, ct);
        return Ok(ApiResponse<Guid>.Ok(id, "Work experience added."));
    }

    [HttpDelete("me/work-experiences/{id:guid}")]
    public async Task<IActionResult> DeleteWorkExperience(Guid id, CancellationToken ct)
    {
        await mediator.Send(new DeleteWorkExperienceCommand(id), ct);
        return Ok(ApiResponse.Ok("Work experience removed."));
    }

    // ── Educations ────────────────────────────────────────────────────────────

    [HttpGet("me/educations")]
    public async Task<IActionResult> GetEducations(CancellationToken ct)
    {
        var result = await mediator.Send(new GetEducationsQuery(), ct);
        return Ok(ApiResponse<List<EducationDto>>.Ok(result, "Educations retrieved."));
    }

    [HttpPost("me/educations")]
    public async Task<IActionResult> AddEducation([FromBody] AddEducationCommand command, CancellationToken ct)
    {
        var id = await mediator.Send(command, ct);
        return Ok(ApiResponse<Guid>.Ok(id, "Education added."));
    }

    [HttpDelete("me/educations/{id:guid}")]
    public async Task<IActionResult> DeleteEducation(Guid id, CancellationToken ct)
    {
        await mediator.Send(new DeleteEducationCommand(id), ct);
        return Ok(ApiResponse.Ok("Education removed."));
    }

    // ── Recruiter view ────────────────────────────────────────────────────────

    [HttpGet]
    public async Task<IActionResult> GetCandidates(
        [FromQuery] string? search,
        [FromQuery] bool? isOpenToWork,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(
            new GetCandidatesQuery(search, isOpenToWork, pageNumber, pageSize), ct);
        return Ok(ApiResponse<PagedList<CandidateSummaryDto>>.Ok(result, "Candidates retrieved."));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new GetCandidateByIdQuery(id), ct);
        if (result is null) return NotFound(ApiResponse.Fail("Candidate profile not found."));
        return Ok(ApiResponse<CandidateProfileDto>.Ok(result, "Candidate profile retrieved."));
    }
}
