using JobPortal.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobPortal.Application.Features.Dashboard.Queries.GetApplicationTrend;

public sealed record GetApplicationTrendQuery(int Days = 30) : IRequest<List<ApplicationTrendPoint>>;

public sealed record ApplicationTrendPoint(string Date, int Count);

public sealed class GetApplicationTrendQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetApplicationTrendQuery, List<ApplicationTrendPoint>>
{
    public async Task<List<ApplicationTrendPoint>> Handle(GetApplicationTrendQuery request, CancellationToken ct)
    {
        var since = DateTimeOffset.UtcNow.AddDays(-request.Days);

        var raw = await db.JobApplications
            .Where(a => a.CreatedOn >= since)
            .GroupBy(a => a.CreatedOn.Date)
            .Select(g => new { Date = g.Key, Count = g.Count() })
            .OrderBy(x => x.Date)
            .ToListAsync(ct);

        return raw.Select(x => new ApplicationTrendPoint(
            x.Date.ToString("yyyy-MM-dd"),
            x.Count)).ToList();
    }
}
