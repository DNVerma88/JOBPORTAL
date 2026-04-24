using JobPortal.API.Models;
using JobPortal.Application.Features.Master.MasterFeatures;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobPortal.API.Controllers.v1;

[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
[Authorize]
public sealed class MasterController(IMediator mediator) : ControllerBase
{
    [HttpGet("skills")]
    [AllowAnonymous]
    public async Task<IActionResult> GetSkills(
        [FromQuery] string? search = null,
        [FromQuery] int limit = 50,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetSkillsQuery(search, limit), ct);
        return Ok(ApiResponse<List<SkillDto>>.Ok(result, "Skills retrieved."));
    }

    [HttpGet("industries")]
    [AllowAnonymous]
    public async Task<IActionResult> GetIndustries(CancellationToken ct)
    {
        var result = await mediator.Send(new GetIndustriesQuery(), ct);
        return Ok(ApiResponse<List<IndustryDto>>.Ok(result, "Industries retrieved."));
    }

    [HttpGet("categories")]
    [AllowAnonymous]
    public async Task<IActionResult> GetCategories(
        [FromQuery] Guid? industryId = null,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetJobCategoriesQuery(industryId), ct);
        return Ok(ApiResponse<List<JobCategoryDto>>.Ok(result, "Categories retrieved."));
    }

    [HttpGet("subscription-plans")]
    [AllowAnonymous]
    public async Task<IActionResult> GetSubscriptionPlans(CancellationToken ct)
    {
        var result = await mediator.Send(new GetSubscriptionPlansQuery(), ct);
        return Ok(ApiResponse<List<SubscriptionPlanDto>>.Ok(result, "Subscription plans retrieved."));
    }

    // ── Skill CRUD (admin) ────────────────────────────────────────────────────

    [HttpPost("skills")]
    public async Task<IActionResult> CreateSkill([FromBody] CreateSkillCommand command, CancellationToken ct)
    {
        var id = await mediator.Send(command, ct);
        return Ok(ApiResponse<Guid>.Ok(id, "Skill created."));
    }

    [HttpPut("skills/{id:guid}")]
    public async Task<IActionResult> UpdateSkill(Guid id, [FromBody] UpdateSkillRequest request, CancellationToken ct)
    {
        await mediator.Send(new UpdateSkillCommand(id, request.Name, request.Category, request.IsActive), ct);
        return Ok(ApiResponse.Ok("Skill updated."));
    }

    [HttpDelete("skills/{id:guid}")]
    public async Task<IActionResult> DeleteSkill(Guid id, CancellationToken ct)
    {
        await mediator.Send(new DeleteSkillCommand(id), ct);
        return Ok(ApiResponse.Ok("Skill deleted."));
    }

    // ── Industry CRUD (admin) ─────────────────────────────────────────────────

    [HttpPost("industries")]
    public async Task<IActionResult> CreateIndustry([FromBody] CreateIndustryRequest request, CancellationToken ct)
    {
        var id = await mediator.Send(new CreateIndustryCommand(request.Name, request.Description, request.SortOrder), ct);
        return Ok(ApiResponse<Guid>.Ok(id, "Industry created."));
    }

    [HttpPut("industries/{id:guid}")]
    public async Task<IActionResult> UpdateIndustry(Guid id, [FromBody] UpdateIndustryRequest request, CancellationToken ct)
    {
        await mediator.Send(new UpdateIndustryCommand(id, request.Name, request.Description, request.SortOrder, request.IsActive), ct);
        return Ok(ApiResponse.Ok("Industry updated."));
    }

    [HttpDelete("industries/{id:guid}")]
    public async Task<IActionResult> DeleteIndustry(Guid id, CancellationToken ct)
    {
        await mediator.Send(new DeleteIndustryCommand(id), ct);
        return Ok(ApiResponse.Ok("Industry deleted."));
    }

    // ── Job Category CRUD (admin) ─────────────────────────────────────────────

    [HttpPost("categories")]
    public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryRequest request, CancellationToken ct)
    {
        var id = await mediator.Send(new CreateJobCategoryCommand(request.Name, request.IndustryId, request.SortOrder), ct);
        return Ok(ApiResponse<Guid>.Ok(id, "Category created."));
    }

    [HttpPut("categories/{id:guid}")]
    public async Task<IActionResult> UpdateCategory(Guid id, [FromBody] UpdateCategoryRequest request, CancellationToken ct)
    {
        await mediator.Send(new UpdateJobCategoryCommand(id, request.Name, request.IndustryId, request.SortOrder, request.IsActive), ct);
        return Ok(ApiResponse.Ok("Category updated."));
    }

    [HttpDelete("categories/{id:guid}")]
    public async Task<IActionResult> DeleteCategory(Guid id, CancellationToken ct)
    {
        await mediator.Send(new DeleteJobCategoryCommand(id), ct);
        return Ok(ApiResponse.Ok("Category deleted."));
    }
}

// -- Request DTOs -----------------------------------------------------------
public sealed record UpdateSkillRequest(string Name, string? Category, bool IsActive);
public sealed record CreateIndustryRequest(string Name, string? Description, int SortOrder);
public sealed record UpdateIndustryRequest(string Name, string? Description, int SortOrder, bool IsActive);
public sealed record CreateCategoryRequest(string Name, Guid? IndustryId, int SortOrder);
public sealed record UpdateCategoryRequest(string Name, Guid? IndustryId, int SortOrder, bool IsActive);
