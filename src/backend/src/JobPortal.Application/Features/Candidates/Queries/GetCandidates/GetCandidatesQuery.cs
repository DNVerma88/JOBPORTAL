using JobPortal.Application.Common.Interfaces;
using JobPortal.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobPortal.Application.Features.Candidates.Queries.GetCandidates;

public sealed record GetCandidatesQuery(
    string? Search = null,
    bool? IsOpenToWork = null,
    int PageNumber = 1,
    int PageSize = 20) : IRequest<PagedList<CandidateSummaryDto>>;

public sealed record CandidateSummaryDto(
    Guid ProfileId,
    Guid UserId,
    string FullName,
    string? Headline,
    string? CurrentJobTitle,
    string? CurrentCompany,
    short? TotalExperienceYears,
    bool IsOpenToWork,
    short ProfileCompletionPct,
    string? ResumeUrl);

public sealed class GetCandidatesQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetCandidatesQuery, PagedList<CandidateSummaryDto>>
{
    public async Task<PagedList<CandidateSummaryDto>> Handle(GetCandidatesQuery request, CancellationToken ct)
    {
        var query =
            from p in db.JobSeekerProfiles
            join u in db.Users on p.UserId equals u.Id into users
            from u in users.DefaultIfEmpty()
            select new { Profile = p, User = u };

        if (request.IsOpenToWork.HasValue)
            query = query.Where(x => x.Profile.IsOpenToWork == request.IsOpenToWork.Value);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim().ToLower();
            query = query.Where(x =>
                (x.User != null && (x.User.FirstName + " " + x.User.LastName).ToLower().Contains(search)) ||
                (x.Profile.Headline != null && x.Profile.Headline.ToLower().Contains(search)) ||
                (x.Profile.CurrentJobTitle != null && x.Profile.CurrentJobTitle.ToLower().Contains(search)) ||
                (x.Profile.CurrentCompany != null && x.Profile.CurrentCompany.ToLower().Contains(search)));
        }

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(x => x.Profile.ProfileCompletionPct)
            .ThenByDescending(x => x.Profile.CreatedOn)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new CandidateSummaryDto(
                x.Profile.Id,
                x.Profile.UserId,
                x.User != null ? x.User.FirstName + " " + x.User.LastName : "Unknown",
                x.Profile.Headline,
                x.Profile.CurrentJobTitle,
                x.Profile.CurrentCompany,
                x.Profile.TotalExperienceYears,
                x.Profile.IsOpenToWork,
                x.Profile.ProfileCompletionPct,
                x.Profile.ResumeUrl))
            .ToListAsync(ct);

        return PagedList<CandidateSummaryDto>.Create(items, total, request.PageNumber, request.PageSize);
    }
}
