using JobPortal.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobPortal.Application.Features.Dashboard.Queries.GetStats;

public sealed record GetDashboardStatsQuery : IRequest<DashboardStatsResponse>;

public sealed record DashboardStatsResponse(
    // Recruiter / Admin stats
    int TotalJobs,
    int ActiveJobs,
    int TotalApplications,
    int PendingApplications,
    int TotalCandidates,
    int TotalCompanies,
    int ScheduledInterviews,
    int PendingOffers,
    int PendingReviews,
    int HiredThisMonth,
    // JobSeeker stats
    int ActiveApplications,
    int SavedJobs,
    int ProfileViews
);

public sealed class GetDashboardStatsQueryHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    : IRequestHandler<GetDashboardStatsQuery, DashboardStatsResponse>
{
    public async Task<DashboardStatsResponse> Handle(GetDashboardStatsQuery request, CancellationToken ct)
    {
        var userId = currentUser.UserId ?? Guid.Empty;
        var isJobSeeker = currentUser.Roles.Contains("JobSeeker");

        if (isJobSeeker)
        {
            // Scoped to the current user
            var totalApps = await db.JobApplications.CountAsync(a => a.ApplicantId == userId, ct);
            var activeApps = await db.JobApplications.CountAsync(
                a => a.ApplicantId == userId &&
                     a.Status != Domain.Enums.ApplicationStatus.Rejected &&
                     a.Status != Domain.Enums.ApplicationStatus.Withdrawn, ct);
            var saved = await db.SavedJobs.CountAsync(s => s.UserId == userId, ct);

            return new DashboardStatsResponse(
                TotalJobs: 0, ActiveJobs: 0,
                TotalApplications: totalApps, PendingApplications: 0,
                TotalCandidates: 0, TotalCompanies: 0,
                ScheduledInterviews: 0, PendingOffers: 0,
                PendingReviews: 0, HiredThisMonth: 0,
                ActiveApplications: activeApps, SavedJobs: saved, ProfileViews: 0);
        }
        else
        {
            // Tenant-level recruiter/admin stats
            var totalJobs = await db.JobPostings.CountAsync(ct);
            var activeJobs = await db.JobPostings.CountAsync(j => j.Status == Domain.Enums.JobPostingStatus.Published, ct);
            var totalApplications = await db.JobApplications.CountAsync(ct);
            var pendingApplications = await db.JobApplications.CountAsync(a => a.Status == Domain.Enums.ApplicationStatus.Pending, ct);
            var pendingReviews = await db.JobApplications.CountAsync(a => a.Status == Domain.Enums.ApplicationStatus.Screening || a.Status == Domain.Enums.ApplicationStatus.Pending, ct);
            var totalCandidates = await db.JobSeekerProfiles.CountAsync(ct);
            var totalCompanies = await db.Companies.CountAsync(ct);
            var scheduledInterviews = await db.InterviewSchedules.CountAsync(i => !i.IsCancelled && i.ScheduledAt >= DateTimeOffset.UtcNow, ct);
            var pendingOffers = await db.OfferLetters.CountAsync(o => o.Status == Domain.Enums.OfferLetterStatus.Sent, ct);
            var monthStart = new DateTimeOffset(DateTimeOffset.UtcNow.Year, DateTimeOffset.UtcNow.Month, 1, 0, 0, 0, TimeSpan.Zero);
            var hiredThisMonth = await db.JobApplications.CountAsync(a => a.Status == Domain.Enums.ApplicationStatus.Hired && a.ModifiedOn >= monthStart, ct);

            return new DashboardStatsResponse(
                TotalJobs: totalJobs, ActiveJobs: activeJobs,
                TotalApplications: totalApplications, PendingApplications: pendingApplications,
                TotalCandidates: totalCandidates, TotalCompanies: totalCompanies,
                ScheduledInterviews: scheduledInterviews, PendingOffers: pendingOffers,
                PendingReviews: pendingReviews, HiredThisMonth: hiredThisMonth,
                ActiveApplications: 0, SavedJobs: 0, ProfileViews: 0);
        }
    }
}
