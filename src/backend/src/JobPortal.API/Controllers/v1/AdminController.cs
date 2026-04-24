using JobPortal.API.Models;
using JobPortal.Application.Common.Models;
using JobPortal.Application.Features.Admin.AdminFeatures;
using JobPortal.Application.Features.Config.ConfigFeatures;
using JobPortal.Application.Features.Master.MasterFeatures;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace JobPortal.API.Controllers.v1;

[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
[Authorize]
public sealed class AdminController(IMediator mediator) : ControllerBase
{
    // ══ USERS ════════════════════════════════════════════════════════════════

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetUsersQuery(pageNumber, pageSize, search), ct);
        return Ok(ApiResponse<PagedList<AdminUserDto>>.Ok(result, "Users retrieved."));
    }

    [HttpPatch("users/{id:guid}/suspend")]
    public async Task<IActionResult> SuspendUser(Guid id, CancellationToken ct)
    {
        await mediator.Send(new SuspendUserCommand(id), ct);
        return Ok(ApiResponse.Ok("User suspended."));
    }

    [HttpPatch("users/{id:guid}/activate")]
    public async Task<IActionResult> ActivateUser(Guid id, CancellationToken ct)
    {
        await mediator.Send(new ActivateUserCommand(id), ct);
        return Ok(ApiResponse.Ok("User activated."));
    }

    // ══ TENANTS ═══════════════════════════════════════════════════════════════

    [HttpGet("tenants")]
    public async Task<IActionResult> GetTenants(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetTenantsQuery(pageNumber, pageSize, search), ct);
        return Ok(ApiResponse<PagedList<TenantDto>>.Ok(result, "Tenants retrieved."));
    }

    [HttpPost("tenants")]
    public async Task<IActionResult> CreateTenant([FromBody] CreateTenantCommand command, CancellationToken ct)
    {
        var id = await mediator.Send(command, ct);
        return Ok(ApiResponse<Guid>.Ok(id, "Tenant created."));
    }

    [HttpPatch("tenants/{id:guid}/status")]
    public async Task<IActionResult> UpdateTenantStatus(Guid id, [FromBody] UpdateTenantStatusRequest request, CancellationToken ct)
    {
        await mediator.Send(new UpdateTenantStatusCommand(id, request.IsActive), ct);
        return Ok(ApiResponse.Ok("Tenant status updated."));
    }

    // ══ ROLES & PERMISSIONS ════════════════════════════════════════════════════

    [HttpGet("roles")]
    public async Task<IActionResult> GetRoles(CancellationToken ct)
    {
        var result = await mediator.Send(new GetRolesQuery(), ct);
        return Ok(ApiResponse<List<RoleDto>>.Ok(result, "Roles retrieved."));
    }

    [HttpGet("roles/{roleId:guid}/permissions")]
    public async Task<IActionResult> GetRolePermissions(Guid roleId, CancellationToken ct)
    {
        var result = await mediator.Send(new GetRolePermissionsQuery(roleId), ct);
        return Ok(ApiResponse<List<PermissionDto>>.Ok(result, "Permissions retrieved."));
    }

    [HttpPut("roles/{roleId:guid}/permissions")]
    public async Task<IActionResult> UpdateRolePermissions(Guid roleId, [FromBody] UpdateRolePermissionsRequest request, CancellationToken ct)
    {
        await mediator.Send(new UpdateRolePermissionsCommand(roleId, request.PermissionIds), ct);
        return Ok(ApiResponse.Ok("Permissions updated."));
    }

    // ══ AUDIT LOGS ════════════════════════════════════════════════════════════

    [HttpGet("audit-logs")]
    public async Task<IActionResult> GetAuditLogs(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50,
        [FromQuery] string? entityType = null,
        [FromQuery] string? action = null,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetAuditLogsQuery(pageNumber, pageSize, entityType, action), ct);
        return Ok(ApiResponse<PagedList<AuditLogDto>>.Ok(result, "Audit logs retrieved."));
    }

    // ══ SESSIONS ══════════════════════════════════════════════════════════════

    [HttpGet("sessions")]
    public async Task<IActionResult> GetSessions(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetSessionsQuery(pageNumber, pageSize), ct);
        return Ok(ApiResponse<PagedList<SessionDto>>.Ok(result, "Sessions retrieved."));
    }

    [HttpDelete("sessions/{id:guid}")]
    public async Task<IActionResult> DeleteSession(Guid id, CancellationToken ct)
    {
        await mediator.Send(new DeleteSessionCommand(id), ct);
        return Ok(ApiResponse.Ok("Session terminated."));
    }

    // ══ SETTINGS ══════════════════════════════════════════════════════════════

    // ══ SETTINGS ══════════════════════════════════════════════════════════════════

    [HttpGet("settings")]
    public async Task<IActionResult> GetAllSettings(CancellationToken ct)
    {
        var result = await mediator.Send(new GetAllSettingsQuery(), ct);
        return Ok(ApiResponse<List<TenantSettingDto>>.Ok(result, "Settings retrieved."));
    }

    [HttpGet("settings/{key}")]
    public async Task<IActionResult> GetSetting(string key, CancellationToken ct)
    {
        var result = await mediator.Send(new GetSettingQuery(key), ct);
        return Ok(ApiResponse<TenantSettingDto?>.Ok(result, "Setting retrieved."));
    }

    [HttpPut("settings/{key}")]
    public async Task<IActionResult> UpdateSetting(string key, [FromBody] UpdateSettingRequest request, CancellationToken ct)
    {
        await mediator.Send(new UpdateSettingCommand(key, request.Value), ct);
        return Ok(ApiResponse.Ok("Setting updated."));
    }

    // ══ EMAIL TEMPLATES ════════════════════════════════════════════════════════

    [HttpGet("email-templates")]
    public async Task<IActionResult> GetEmailTemplates(
        [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetEmailTemplatesQuery(pageNumber, pageSize), ct);
        return Ok(ApiResponse<PagedList<EmailTemplateDto>>.Ok(result, "Email templates retrieved."));
    }

    [HttpPut("email-templates/{id:guid}")]
    public async Task<IActionResult> UpdateEmailTemplate(Guid id, [FromBody] UpdateEmailTemplateRequest request, CancellationToken ct)
    {
        await mediator.Send(new UpdateEmailTemplateCommand(id, request.Subject, request.BodyHtml, request.BodyText), ct);
        return Ok(ApiResponse.Ok("Email template updated."));
    }

    // ══ FEATURE FLAGS ══════════════════════════════════════════════════════════

    [HttpGet("feature-flags")]
    public async Task<IActionResult> GetFeatureFlags(CancellationToken ct)
    {
        var result = await mediator.Send(new GetFeatureFlagsQuery(), ct);
        return Ok(ApiResponse<List<FeatureFlagDto>>.Ok(result, "Feature flags retrieved."));
    }

    [HttpPatch("feature-flags/{id:guid}")]
    public async Task<IActionResult> ToggleFeatureFlag(Guid id, [FromBody] ToggleFeatureFlagRequest request, CancellationToken ct)
    {
        await mediator.Send(new ToggleFeatureFlagCommand(id, request.IsEnabled), ct);
        return Ok(ApiResponse.Ok("Feature flag updated."));
    }

    // ══ ANNOUNCEMENTS ══════════════════════════════════════════════════════════

    [HttpGet("announcements")]
    public async Task<IActionResult> GetAnnouncements(
        [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetAnnouncementsQuery(pageNumber, pageSize), ct);
        return Ok(ApiResponse<PagedList<AnnouncementDto>>.Ok(result, "Announcements retrieved."));
    }

    [HttpPost("announcements")]
    public async Task<IActionResult> CreateAnnouncement([FromBody] CreateAnnouncementCommand command, CancellationToken ct)
    {
        var id = await mediator.Send(command, ct);
        return Ok(ApiResponse<Guid>.Ok(id, "Announcement created."));
    }

    [HttpDelete("announcements/{id:guid}")]
    public async Task<IActionResult> DeleteAnnouncement(Guid id, CancellationToken ct)
    {
        await mediator.Send(new DeleteAnnouncementCommand(id), ct);
        return Ok(ApiResponse.Ok("Announcement deleted."));
    }

    // ══ SUBSCRIPTION PLANS ════════════════════════════════════════════════════

    [HttpGet("subscription-plans")]
    public async Task<IActionResult> GetSubscriptionPlans(CancellationToken ct)
    {
        var result = await mediator.Send(new GetSubscriptionPlansQuery(), ct);
        return Ok(ApiResponse<List<SubscriptionPlanDto>>.Ok(result, "Subscription plans retrieved."));
    }

    [HttpPost("subscription-plans")]
    public async Task<IActionResult> CreateSubscriptionPlan([FromBody] CreateSubscriptionPlanCommand command, CancellationToken ct)
    {
        var id = await mediator.Send(command, ct);
        return Ok(ApiResponse<Guid>.Ok(id, "Subscription plan created."));
    }

    [HttpPut("subscription-plans/{id:guid}")]
    public async Task<IActionResult> UpdateSubscriptionPlan(Guid id, [FromBody] UpdateSubscriptionPlanRequest request, CancellationToken ct)
    {
        await mediator.Send(new UpdateSubscriptionPlanCommand(id, request.Tier, request.Name, request.Description,
            request.PriceMonthly, request.PriceAnnually, request.CurrencyCode, request.MaxJobPostings,
            request.MaxUsers, request.MaxResumeViews, request.JobPostingDurationDays, request.IsActive, request.SortOrder), ct);
        return Ok(ApiResponse.Ok("Subscription plan updated."));
    }

    // ══ PERMISSIONS (ALL) ═════════════════════════════════════════════════════════

    [HttpGet("permissions")]
    public async Task<IActionResult> GetPermissions(CancellationToken ct)
    {
        var result = await mediator.Send(new GetAllPermissionsQuery(), ct);
        return Ok(ApiResponse<List<PermissionDto>>.Ok(result, "Permissions retrieved."));
    }

    // ══ CREATE USER ═══════════════════════════════════════════════════════════════

    [HttpPost("users")]
    public async Task<IActionResult> CreateUser([FromBody] CreateAdminUserCommand command, CancellationToken ct)
    {
        var id = await mediator.Send(command, ct);
        return Ok(ApiResponse<Guid>.Ok(id, "User created."));
    }

    [HttpPut("users/{id:guid}")]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateAdminUserRequest request, CancellationToken ct)
    {
        await mediator.Send(new UpdateAdminUserCommand(id, request.FirstName, request.LastName, request.RoleId), ct);
        return Ok(ApiResponse.Ok("User updated."));
    }

    [HttpPost("users/{id:guid}/reset-password")]
    public async Task<IActionResult> ResetUserPassword(Guid id, CancellationToken ct)
    {
        await mediator.Send(new AdminResetUserPasswordCommand(id), ct);
        return Ok(ApiResponse.Ok("Password reset email sent."));
    }

    // ╔═ ROLE CRUD ═════════════════════════════════════════════════════════════

    [HttpPost("roles")]
    public async Task<IActionResult> CreateRole([FromBody] CreateRoleRequest request, CancellationToken ct)
    {
        var id = await mediator.Send(new CreateRoleCommand(request.Name, request.Description), ct);
        return Ok(ApiResponse<Guid>.Ok(id, "Role created."));
    }

    [HttpPut("roles/{id:guid}")]
    public async Task<IActionResult> UpdateRole(Guid id, [FromBody] UpdateRoleRequest request, CancellationToken ct)
    {
        await mediator.Send(new UpdateRoleCommand(id, request.Name, request.Description), ct);
        return Ok(ApiResponse.Ok("Role updated."));
    }

    [HttpDelete("roles/{id:guid}")]
    public async Task<IActionResult> DeleteRole(Guid id, CancellationToken ct)
    {
        await mediator.Send(new DeleteRoleCommand(id), ct);
        return Ok(ApiResponse.Ok("Role deleted."));
    }

    // ╔═ TENANT EDIT ═══════════════════════════════════════════════════════════

    [HttpPut("tenants/{id:guid}")]
    public async Task<IActionResult> UpdateTenant(Guid id, [FromBody] UpdateTenantRequest request, CancellationToken ct)
    {
        await mediator.Send(new UpdateTenantCommand(id, request.Name, request.CustomDomain), ct);
        return Ok(ApiResponse.Ok("Tenant updated."));
    }

    // ╔═ ANNOUNCEMENTS EDIT ═══════════════════════════════════════════════════

    [HttpPut("announcements/{id:guid}")]
    public async Task<IActionResult> UpdateAnnouncement(Guid id, [FromBody] UpdateAnnouncementRequest request, CancellationToken ct)
    {
        await mediator.Send(new UpdateAnnouncementCommand(id, request.Title, request.Body, request.Type,
            request.IsGlobal, request.IsActive, request.StartsAt, request.EndsAt), ct);
        return Ok(ApiResponse.Ok("Announcement updated."));
    }

    // ╔═ SUBSCRIPTION PLANS DELETE ════════════════════════════════════════════

    [HttpDelete("subscription-plans/{id:guid}")]
    public async Task<IActionResult> DeleteSubscriptionPlan(Guid id, CancellationToken ct)
    {
        await mediator.Send(new DeleteSubscriptionPlanCommand(id), ct);
        return Ok(ApiResponse.Ok("Subscription plan deleted."));
    }
}

// -- Request DTOs -------------------------------------------------------
public sealed record UpdateTenantStatusRequest(bool IsActive);
public sealed record UpdateRolePermissionsRequest(List<Guid> PermissionIds);
public sealed record UpdateSettingRequest(string Value);
public sealed record UpdateEmailTemplateRequest(string Subject, string BodyHtml, string? BodyText);
public sealed record ToggleFeatureFlagRequest(bool IsEnabled);
public sealed record UpdateSubscriptionPlanRequest(
    string Tier, string Name, string? Description,
    decimal PriceMonthly, decimal PriceAnnually, string CurrencyCode,
    int? MaxJobPostings, int? MaxUsers, int? MaxResumeViews,
    int JobPostingDurationDays, bool IsActive, int SortOrder);
public sealed record UpdateAdminUserRequest(string FirstName, string LastName, Guid? RoleId);
public sealed record CreateRoleRequest(string Name, string? Description);
public sealed record UpdateRoleRequest(string Name, string? Description);
public sealed record UpdateTenantRequest(string Name, string? CustomDomain);
public sealed record UpdateAnnouncementRequest(
    string Title, string Body, string Type, bool IsGlobal, bool IsActive,
    DateTimeOffset StartsAt, DateTimeOffset? EndsAt);
