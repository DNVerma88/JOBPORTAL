using FluentValidation;
using JobPortal.Application.Common.Interfaces;
using JobPortal.Application.Common.Models;
using JobPortal.Domain.Entities.Portal;
using JobPortal.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobPortal.Application.Features.JobAlerts.JobAlertFeatures;

// ── Get Job Alerts ────────────────────────────────────────────────────────────
public sealed record GetJobAlertsQuery(int PageNumber = 1, int PageSize = 20)
    : IRequest<PagedList<JobAlertDto>>;

public sealed record JobAlertDto(
    Guid Id, string Name, string Frequency, bool IsActive, DateTimeOffset? LastSentAt, DateTimeOffset CreatedOn,
    string? Keyword = null, string? JobType = null, string? WorkMode = null, string? ExperienceLevel = null);

public sealed class GetJobAlertsQueryHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    : IRequestHandler<GetJobAlertsQuery, PagedList<JobAlertDto>>
{
    public async Task<PagedList<JobAlertDto>> Handle(GetJobAlertsQuery request, CancellationToken ct)
    {
        var userId = currentUser.UserId ?? Guid.Empty;
        var query = db.JobAlerts.Where(a => a.UserId == userId);

        var total = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(a => a.CreatedOn)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(ct);

        var dtos = items.Select(a => new JobAlertDto(a.Id, a.Name, a.Frequency.ToString(), a.IsActive, a.LastSentAt, a.CreatedOn,
            a.Keywords?.Length > 0 ? a.Keywords[0] : null,
            a.JobTypes?.Length > 0 ? a.JobTypes[0] : null,
            a.WorkModes?.Length > 0 ? a.WorkModes[0] : null,
            a.ExperienceLevels?.Length > 0 ? a.ExperienceLevels[0] : null)).ToList();

        return PagedList<JobAlertDto>.Create(dtos, total, request.PageNumber, request.PageSize);
    }
}

// ── Create Job Alert ──────────────────────────────────────────────────────────
public sealed record CreateJobAlertCommand(
    string? Name, AlertFrequency Frequency,
    string? Keyword = null, string? Location = null,
    string? JobType = null, string? WorkMode = null,
    string? ExperienceLevel = null) : IRequest<Guid>;

public sealed class CreateJobAlertCommandValidator : AbstractValidator<CreateJobAlertCommand>
{
    public CreateJobAlertCommandValidator()
    {
        RuleFor(x => x.Name).MaximumLength(200).When(x => x.Name is not null);
    }
}

public sealed class CreateJobAlertCommandHandler(
    IApplicationDbContext db,
    ICurrentUserService currentUser)
    : IRequestHandler<CreateJobAlertCommand, Guid>
{
    public async Task<Guid> Handle(CreateJobAlertCommand request, CancellationToken ct)
    {
        var userId = currentUser.UserId ?? Guid.Empty;
        var tenantId = currentUser.TenantId ?? Guid.Empty;
        var name = request.Name ?? request.Keyword ?? "My Job Alert";

        var alert = JobAlert.Create(tenantId, userId, name, request.Frequency, userId);
        var keywords = request.Keyword is not null ? new[] { request.Keyword } : null;
        var jobTypes = request.JobType is not null ? new[] { request.JobType } : null;
        var workModes = request.WorkMode is not null ? new[] { request.WorkMode } : null;
        var experienceLevels = request.ExperienceLevel is not null ? new[] { request.ExperienceLevel } : null;
        alert.UpdateFilters(keywords, null, null, jobTypes, workModes, null, null, experienceLevels, request.Frequency, userId);

        db.JobAlerts.Add(alert);
        await db.SaveChangesAsync(ct);
        return alert.Id;
    }
}

// ── Toggle Job Alert ──────────────────────────────────────────────────────────
public sealed record ToggleJobAlertCommand(Guid Id) : IRequest;

public sealed class ToggleJobAlertCommandHandler(
    IApplicationDbContext db,
    ICurrentUserService currentUser)
    : IRequestHandler<ToggleJobAlertCommand>
{
    public async Task Handle(ToggleJobAlertCommand request, CancellationToken ct)
    {
        var userId = currentUser.UserId ?? Guid.Empty;
        var alert = await db.JobAlerts.FirstOrDefaultAsync(a => a.Id == request.Id, ct)
            ?? throw new KeyNotFoundException($"Job alert {request.Id} not found.");
        alert.Toggle(userId);
        await db.SaveChangesAsync(ct);
    }
}

// ── Delete Job Alert ──────────────────────────────────────────────────────────
public sealed record DeleteJobAlertCommand(Guid Id) : IRequest;

public sealed class DeleteJobAlertCommandHandler(
    IApplicationDbContext db,
    ICurrentUserService currentUser)
    : IRequestHandler<DeleteJobAlertCommand>
{
    public async Task Handle(DeleteJobAlertCommand request, CancellationToken ct)
    {
        var userId = currentUser.UserId ?? Guid.Empty;
        var alert = await db.JobAlerts.FirstOrDefaultAsync(a => a.Id == request.Id, ct)
            ?? throw new KeyNotFoundException($"Job alert {request.Id} not found.");
        alert.SoftDelete(userId);
        await db.SaveChangesAsync(ct);
    }
}

// ── Update Job Alert ──────────────────────────────────────────────────────────
public sealed record UpdateJobAlertCommand(
    Guid Id, AlertFrequency Frequency,
    string? Keyword = null, string? JobType = null,
    string? WorkMode = null, string? ExperienceLevel = null) : IRequest;

public sealed class UpdateJobAlertCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    : IRequestHandler<UpdateJobAlertCommand>
{
    public async Task Handle(UpdateJobAlertCommand request, CancellationToken ct)
    {
        var userId = currentUser.UserId ?? Guid.Empty;
        var alert = await db.JobAlerts.FirstOrDefaultAsync(a => a.Id == request.Id, ct)
            ?? throw new KeyNotFoundException($"Job alert {request.Id} not found.");

        var keywords = request.Keyword is not null ? new[] { request.Keyword } : null;
        var jobTypes = request.JobType is not null ? new[] { request.JobType } : null;
        var workModes = request.WorkMode is not null ? new[] { request.WorkMode } : null;
        var experienceLevels = request.ExperienceLevel is not null ? new[] { request.ExperienceLevel } : null;
        alert.UpdateFilters(keywords, null, null, jobTypes, workModes, null, null, experienceLevels, request.Frequency, userId);
        await db.SaveChangesAsync(ct);
    }
}
