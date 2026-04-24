using JobPortal.API.Models;
using JobPortal.Application.Common.Models;
using JobPortal.Application.Features.Config.ConfigFeatures;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobPortal.API.Controllers.v1;

[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
[Authorize]
public sealed class ConfigController(IMediator mediator) : ControllerBase
{
    // ── Tenant Settings ───────────────────────────────────────────────────

    [HttpGet("settings")]
    public async Task<IActionResult> GetSetting([FromQuery] string key, CancellationToken ct)
    {
        var result = await mediator.Send(new GetSettingQuery(key), ct);
        return Ok(ApiResponse<TenantSettingDto?>.Ok(result, "Setting retrieved."));
    }

    [HttpPut("settings")]
    public async Task<IActionResult> UpdateSetting([FromBody] UpdateSettingCommand command, CancellationToken ct)
    {
        await mediator.Send(command, ct);
        return Ok(ApiResponse.Ok("Setting updated."));
    }

    // ── Email Templates ───────────────────────────────────────────────────

    [HttpGet("email-templates")]
    public async Task<IActionResult> GetEmailTemplates(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetEmailTemplatesQuery(pageNumber, pageSize), ct);
        return Ok(ApiResponse<PagedList<EmailTemplateDto>>.Ok(result, "Email templates retrieved."));
    }

    [HttpPut("email-templates/{id:guid}")]
    public async Task<IActionResult> UpdateEmailTemplate(Guid id, [FromBody] UpdateEmailTemplateBody request, CancellationToken ct)
    {
        await mediator.Send(new UpdateEmailTemplateCommand(id, request.Subject, request.BodyHtml, request.BodyText), ct);
        return Ok(ApiResponse.Ok("Email template updated."));
    }

    // ── Feature Flags ─────────────────────────────────────────────────────

    [HttpGet("feature-flags")]
    public async Task<IActionResult> GetFeatureFlags(CancellationToken ct)
    {
        var result = await mediator.Send(new GetFeatureFlagsQuery(), ct);
        return Ok(ApiResponse<List<FeatureFlagDto>>.Ok(result, "Feature flags retrieved."));
    }

    [HttpPatch("feature-flags/{id:guid}/toggle")]
    public async Task<IActionResult> ToggleFeatureFlag(Guid id, [FromBody] ToggleFlagRequest request, CancellationToken ct)
    {
        await mediator.Send(new ToggleFeatureFlagCommand(id, request.IsEnabled), ct);
        return Ok(ApiResponse.Ok("Feature flag toggled."));
    }

    // ── Announcements ─────────────────────────────────────────────────────

    [HttpGet("announcements")]
    public async Task<IActionResult> GetAnnouncements(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetAnnouncementsQuery(pageNumber, pageSize), ct);
        return Ok(ApiResponse<PagedList<AnnouncementDto>>.Ok(result, "Announcements retrieved."));
    }

    [HttpPost("announcements")]
    public async Task<IActionResult> CreateAnnouncement([FromBody] CreateAnnouncementCommand command, CancellationToken ct)
    {
        var id = await mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetAnnouncements), ApiResponse<Guid>.Ok(id, "Announcement created."));
    }

    [HttpDelete("announcements/{id:guid}")]
    public async Task<IActionResult> DeleteAnnouncement(Guid id, CancellationToken ct)
    {
        await mediator.Send(new DeleteAnnouncementCommand(id), ct);
        return Ok(ApiResponse.Ok("Announcement deleted."));
    }
}

public sealed record UpdateEmailTemplateBody(string Subject, string BodyHtml, string? BodyText);
public sealed record ToggleFlagRequest(bool IsEnabled);
