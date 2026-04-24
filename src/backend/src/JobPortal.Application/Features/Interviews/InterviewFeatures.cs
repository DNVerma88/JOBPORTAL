using FluentValidation;
using JobPortal.Application.Common.Interfaces;
using JobPortal.Application.Common.Models;
using JobPortal.Domain.Entities.Portal;
using JobPortal.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobPortal.Application.Features.Interviews.Commands;

// ── Get Interviews ────────────────────────────────────────────────────────────
public sealed record GetInterviewsQuery(int PageNumber = 1, int PageSize = 20, Guid? ApplicationId = null)
    : IRequest<PagedList<InterviewDto>>;

public sealed record InterviewDto(
    Guid Id, Guid ApplicationId, string Title, DateTimeOffset ScheduledAt,
    short DurationMinutes, string InterviewType, string? MeetingLink, string? Location,
    bool IsConfirmed, bool IsCancelled, DateTimeOffset CreatedOn,
    string? CandidateName, string? CandidateEmail, string? JobTitle, string Status);

public sealed class GetInterviewsQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetInterviewsQuery, PagedList<InterviewDto>>
{
    public async Task<PagedList<InterviewDto>> Handle(GetInterviewsQuery request, CancellationToken ct)
    {
        var query = db.InterviewSchedules.AsQueryable();
        if (request.ApplicationId.HasValue)
            query = query.Where(i => i.ApplicationId == request.ApplicationId.Value);

        var total = await query.CountAsync(ct);

        var interviews = await query
            .OrderByDescending(i => i.ScheduledAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(ct);

        var appIds = interviews.Select(i => i.ApplicationId).Distinct().ToList();
        var applications = await db.JobApplications
            .Where(a => appIds.Contains(a.Id))
            .ToListAsync(ct);

        var userIds = applications.Select(a => a.ApplicantId).Distinct().ToList();
        var users = await db.Users
            .Where(u => userIds.Contains(u.Id))
            .Select(u => new { u.Id, u.FirstName, u.LastName, u.Email })
            .ToListAsync(ct);

        var jobIds = applications.Select(a => a.JobPostingId).Distinct().ToList();
        var jobs = await db.JobPostings
            .Where(j => jobIds.Contains(j.Id))
            .Select(j => new { j.Id, j.Title })
            .ToListAsync(ct);

        var appMap = applications.ToDictionary(a => a.Id);
        var userMap = users.ToDictionary(u => u.Id);
        var jobMap = jobs.ToDictionary(j => j.Id);

        var items = interviews.Select(i =>
        {
            appMap.TryGetValue(i.ApplicationId, out var app);
            var user = app is not null && userMap.TryGetValue(app.ApplicantId, out var u) ? u : null;
            var job = app is not null && jobMap.TryGetValue(app.JobPostingId, out var j) ? j : null;
            var status = i.IsCancelled ? "Cancelled" : i.IsConfirmed ? "Confirmed" : "Scheduled";
            return new InterviewDto(i.Id, i.ApplicationId, i.Title, i.ScheduledAt,
                i.DurationMinutes, i.InterviewType.ToString(), i.MeetingLink, i.Location,
                i.IsConfirmed, i.IsCancelled, i.CreatedOn,
                user is not null ? $"{user.FirstName} {user.LastName}" : null,
                user?.Email, job?.Title, status);
        }).ToList();

        return PagedList<InterviewDto>.Create(items, total, request.PageNumber, request.PageSize);
    }
}

// ── Schedule Interview ────────────────────────────────────────────────────────
public sealed record ScheduleInterviewCommand(
    Guid ApplicationId,
    string Title,
    string? Description,
    DateTimeOffset ScheduledAt,
    short DurationMinutes,
    InterviewType InterviewType,
    string? MeetingLink,
    string? Location
) : IRequest<Guid>;

public sealed class ScheduleInterviewCommandValidator : AbstractValidator<ScheduleInterviewCommand>
{
    public ScheduleInterviewCommandValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.ApplicationId).NotEmpty();
        RuleFor(x => x.ScheduledAt).GreaterThan(DateTimeOffset.UtcNow);
        RuleFor(x => x.DurationMinutes).GreaterThan((short)0);
    }
}

public sealed class ScheduleInterviewCommandHandler(
    IApplicationDbContext db,
    ICurrentUserService currentUser)
    : IRequestHandler<ScheduleInterviewCommand, Guid>
{
    public async Task<Guid> Handle(ScheduleInterviewCommand request, CancellationToken ct)
    {
        var userId = currentUser.UserId ?? Guid.Empty;
        var tenantId = currentUser.TenantId ?? Guid.Empty;

        var interview = InterviewSchedule.Create(tenantId, request.ApplicationId,
            request.Title, request.ScheduledAt, request.DurationMinutes,
            request.InterviewType, request.MeetingLink, request.Location, request.Description, userId);

        db.InterviewSchedules.Add(interview);
        await db.SaveChangesAsync(ct);
        return interview.Id;
    }
}

// ── Update Interview Status ────────────────────────────────────────────────────
public sealed record UpdateInterviewStatusCommand(Guid Id, bool IsCancelled, string? CancelledReason) : IRequest;

public sealed class UpdateInterviewStatusCommandHandler(
    IApplicationDbContext db,
    ICurrentUserService currentUser)
    : IRequestHandler<UpdateInterviewStatusCommand>
{
    public async Task Handle(UpdateInterviewStatusCommand request, CancellationToken ct)
    {
        var userId = currentUser.UserId ?? Guid.Empty;
        var interview = await db.InterviewSchedules.FirstOrDefaultAsync(i => i.Id == request.Id, ct)
            ?? throw new KeyNotFoundException($"Interview {request.Id} not found.");

        interview.UpdateStatus(request.IsCancelled, request.CancelledReason, userId);
        await db.SaveChangesAsync(ct);
    }
}

// ── Reschedule Interview ────────────────────────────────────────────────────────
public sealed record RescheduleInterviewCommand(
    Guid Id, DateTimeOffset ScheduledAt, short DurationMinutes,
    string? MeetingLink, string? Location) : IRequest;

public sealed class RescheduleInterviewCommandValidator : AbstractValidator<RescheduleInterviewCommand>
{
    public RescheduleInterviewCommandValidator()
    {
        RuleFor(x => x.DurationMinutes).GreaterThan((short)0);
    }
}

public sealed class RescheduleInterviewCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    : IRequestHandler<RescheduleInterviewCommand>
{
    public async Task Handle(RescheduleInterviewCommand request, CancellationToken ct)
    {
        var userId = currentUser.UserId ?? Guid.Empty;
        var interview = await db.InterviewSchedules.FirstOrDefaultAsync(i => i.Id == request.Id, ct)
            ?? throw new KeyNotFoundException($"Interview {request.Id} not found.");
        interview.Reschedule(request.ScheduledAt, request.DurationMinutes, request.MeetingLink, request.Location, userId);
        await db.SaveChangesAsync(ct);
    }
}
