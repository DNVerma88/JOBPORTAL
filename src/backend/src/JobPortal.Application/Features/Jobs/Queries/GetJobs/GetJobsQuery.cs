using JobPortal.Application.Common.Interfaces;
using JobPortal.Application.Common.Models;
using JobPortal.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobPortal.Application.Features.Jobs.Queries.GetJobs;

public sealed record GetJobsQuery(
    int PageNumber = 1,
    int PageSize = 20,
    string? Search = null,
    string? Status = null,
    Guid? CompanyId = null,
    bool MineOnly = false
) : IRequest<PagedList<JobSummaryDto>>;

public sealed record JobSummaryDto(
    Guid Id,
    string Title,
    string Slug,
    Guid CompanyId,
    string? CompanyName,
    string JobType,
    string WorkMode,
    string ExperienceLevel,
    string Status,
    int ApplicationsCount,
    int OpeningsCount,
    bool IsUrgent,
    bool IsFeatured,
    bool IsRemote,
    DateTimeOffset? PublishedAt,
    DateTimeOffset? ExpiresAt,
    DateTimeOffset CreatedOn
);

public sealed class GetJobsQueryHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    : IRequestHandler<GetJobsQuery, PagedList<JobSummaryDto>>
{
    public async Task<PagedList<JobSummaryDto>> Handle(GetJobsQuery request, CancellationToken ct)
    {
        var query = db.JobPostings.AsQueryable();

        if (request.MineOnly)
        {
            var userId = currentUser.UserId ?? Guid.Empty;
            query = query.Where(j => j.CreatedBy == userId);
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
            query = query.Where(j => j.Title.Contains(request.Search));

        if (!string.IsNullOrWhiteSpace(request.Status) &&
            Enum.TryParse<JobPostingStatus>(request.Status, true, out var statusEnum))
            query = query.Where(j => j.Status == statusEnum);

        if (request.CompanyId.HasValue)
            query = query.Where(j => j.CompanyId == request.CompanyId.Value);

        var total = await query.CountAsync(ct);
        var skip = (request.PageNumber - 1) * request.PageSize;

        var items = await query
            .OrderByDescending(j => j.CreatedOn)
            .Skip(skip)
            .Take(request.PageSize)
            .Join(db.Companies,
                j => j.CompanyId,
                c => c.Id,
                (j, c) => new JobSummaryDto(
                    j.Id, j.Title, j.Slug, j.CompanyId, c.Name,
                    j.JobType.ToString(), j.WorkMode.ToString(), j.ExperienceLevel.ToString(),
                    j.Status.ToString(), j.ApplicationsCount, j.OpeningsCount,
                    j.IsUrgent, j.IsFeatured, j.IsRemote,
                    j.PublishedAt, j.ExpiresAt, j.CreatedOn))
            .ToListAsync(ct);

        return PagedList<JobSummaryDto>.Create(items, total, request.PageNumber, request.PageSize);
    }
}
