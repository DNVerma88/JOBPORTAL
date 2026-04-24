using JobPortal.Application.Common.Interfaces;
using JobPortal.Application.Common.Models;
using JobPortal.Application.Features.Jobs.Queries.GetJobs;
using JobPortal.Domain.Entities.Portal;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobPortal.Application.Features.SavedJobs.SavedJobFeatures;

// ── Get Saved Jobs ────────────────────────────────────────────────────────────
public sealed record GetSavedJobsQuery(int PageNumber = 1, int PageSize = 20)
    : IRequest<PagedList<JobSummaryDto>>;

public sealed class GetSavedJobsQueryHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    : IRequestHandler<GetSavedJobsQuery, PagedList<JobSummaryDto>>
{
    public async Task<PagedList<JobSummaryDto>> Handle(GetSavedJobsQuery request, CancellationToken ct)
    {
        var userId = currentUser.UserId ?? Guid.Empty;
        var query = db.SavedJobs.Where(s => s.UserId == userId);

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(s => s.CreatedOn)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Join(db.JobPostings, s => s.JobPostingId, j => j.Id, (s, j) => new { s, j })
            .Join(db.Companies, x => x.j.CompanyId, c => c.Id,
                (x, c) => new JobSummaryDto(
                    x.j.Id, x.j.Title, x.j.Slug, x.j.CompanyId, c.Name,
                    x.j.JobType.ToString(), x.j.WorkMode.ToString(), x.j.ExperienceLevel.ToString(),
                    x.j.Status.ToString(), x.j.ApplicationsCount, x.j.OpeningsCount,
                    x.j.IsUrgent, x.j.IsFeatured, x.j.IsRemote,
                    x.j.PublishedAt, x.j.ExpiresAt, x.j.CreatedOn))
            .ToListAsync(ct);

        return PagedList<JobSummaryDto>.Create(items, total, request.PageNumber, request.PageSize);
    }
}

// ── Save Job ──────────────────────────────────────────────────────────────────
public sealed record SaveJobCommand(Guid JobPostingId) : IRequest<Guid>;

public sealed class SaveJobCommandHandler(
    IApplicationDbContext db,
    ICurrentUserService currentUser)
    : IRequestHandler<SaveJobCommand, Guid>
{
    public async Task<Guid> Handle(SaveJobCommand request, CancellationToken ct)
    {
        var userId = currentUser.UserId ?? Guid.Empty;
        var tenantId = currentUser.TenantId ?? Guid.Empty;

        var existing = await db.SavedJobs
            .FirstOrDefaultAsync(s => s.UserId == userId && s.JobPostingId == request.JobPostingId, ct);
        if (existing is not null) return existing.Id;

        var saved = SavedJob.Create(tenantId, userId, request.JobPostingId, userId);
        db.SavedJobs.Add(saved);
        await db.SaveChangesAsync(ct);
        return saved.Id;
    }
}

// ── Unsave Job (by SavedJob Id) ──────────────────────────────────────────────
public sealed record UnsaveJobCommand(Guid Id) : IRequest;

public sealed class UnsaveJobCommandHandler(
    IApplicationDbContext db,
    ICurrentUserService currentUser)
    : IRequestHandler<UnsaveJobCommand>
{
    public async Task Handle(UnsaveJobCommand request, CancellationToken ct)
    {
        var userId = currentUser.UserId ?? Guid.Empty;
        var saved = await db.SavedJobs.FirstOrDefaultAsync(s => s.Id == request.Id, ct)
            ?? throw new KeyNotFoundException($"Saved job {request.Id} not found.");
        saved.SoftDelete(userId);
        await db.SaveChangesAsync(ct);
    }
}

// ── Unsave Job (by JobPosting Id) ────────────────────────────────────────────
public sealed record UnsaveByJobPostingIdCommand(Guid JobPostingId) : IRequest;

public sealed class UnsaveByJobPostingIdCommandHandler(
    IApplicationDbContext db,
    ICurrentUserService currentUser)
    : IRequestHandler<UnsaveByJobPostingIdCommand>
{
    public async Task Handle(UnsaveByJobPostingIdCommand request, CancellationToken ct)
    {
        var userId = currentUser.UserId ?? Guid.Empty;
        var saved = await db.SavedJobs.FirstOrDefaultAsync(s => s.UserId == userId && s.JobPostingId == request.JobPostingId, ct);
        if (saved is null) return; // idempotent
        saved.SoftDelete(userId);
        await db.SaveChangesAsync(ct);
    }
}
