using JobPortal.Application.Common.Interfaces;
using JobPortal.Application.Common.Models;
using JobPortal.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobPortal.Application.Features.Applications.Queries.GetApplications;

public sealed record GetApplicationsQuery(
    int PageNumber = 1,
    int PageSize = 20,
    Guid? JobPostingId = null,
    string? Status = null,
    Guid? ApplicantId = null
) : IRequest<PagedList<ApplicationSummaryDto>>;

public sealed record ApplicationSummaryDto(
    Guid Id,
    Guid JobPostingId,
    string? JobTitle,
    Guid ApplicantId,
    string? ApplicantName,
    string Status,
    string? AppliedVia,
    bool IsViewed,
    DateTimeOffset CreatedOn
);

public sealed class GetApplicationsQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetApplicationsQuery, PagedList<ApplicationSummaryDto>>
{
    public async Task<PagedList<ApplicationSummaryDto>> Handle(GetApplicationsQuery request, CancellationToken ct)
    {
        var query = db.JobApplications.AsQueryable();

        if (request.JobPostingId.HasValue)
            query = query.Where(a => a.JobPostingId == request.JobPostingId.Value);

        if (request.ApplicantId.HasValue)
            query = query.Where(a => a.ApplicantId == request.ApplicantId.Value);

        if (!string.IsNullOrWhiteSpace(request.Status) &&
            Enum.TryParse<ApplicationStatus>(request.Status, true, out var statusEnum))
            query = query.Where(a => a.Status == statusEnum);

        var total = await query.CountAsync(ct);
        var skip = (request.PageNumber - 1) * request.PageSize;

        var items = await query
            .OrderByDescending(a => a.CreatedOn)
            .Skip(skip)
            .Take(request.PageSize)
            .Select(a => new ApplicationSummaryDto(
                a.Id, a.JobPostingId,
                db.JobPostings.Where(j => j.Id == a.JobPostingId).Select(j => j.Title).FirstOrDefault(),
                a.ApplicantId,
                db.Users.Where(u => u.Id == a.ApplicantId).Select(u => u.FirstName + " " + u.LastName).FirstOrDefault(),
                a.Status.ToString(), a.AppliedVia, a.IsViewed, a.CreatedOn))
            .ToListAsync(ct);

        return PagedList<ApplicationSummaryDto>.Create(items, total, request.PageNumber, request.PageSize);
    }
}
