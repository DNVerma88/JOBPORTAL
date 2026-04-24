using JobPortal.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace JobPortal.Application.Features.Applications.Queries.GetApplicationDetail;

public sealed record GetApplicationDetailQuery(Guid Id) : IRequest<ApplicationDetailDto?>;

public sealed record ApplicationDetailDto(
    Guid Id,
    Guid JobPostingId,
    string? JobTitle,
    string? CompanyName,
    Guid ApplicantId,
    string? ApplicantName,
    string? ApplicantEmail,
    string? CoverLetter,
    decimal? ExpectedSalary,
    Guid? ResumeId,
    string? AppliedVia,
    string Status,
    bool IsViewed,
    DateTimeOffset? ViewedAt,
    DateTimeOffset CreatedOn
);

public sealed class GetApplicationDetailQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetApplicationDetailQuery, ApplicationDetailDto?>
{
    public async Task<ApplicationDetailDto?> Handle(GetApplicationDetailQuery request, CancellationToken ct)
    {
        var app = await db.JobApplications.FirstOrDefaultAsync(a => a.Id == request.Id, ct);
        if (app is null) return null;

        var job = await db.JobPostings.FirstOrDefaultAsync(j => j.Id == app.JobPostingId, ct);
        var company = job is null ? null : await db.Companies.FirstOrDefaultAsync(c => c.Id == job.CompanyId, ct);
        var applicant = await db.Users.FirstOrDefaultAsync(u => u.Id == app.ApplicantId, ct);

        return new ApplicationDetailDto(
            app.Id,
            app.JobPostingId,
            job?.Title,
            company?.Name,
            app.ApplicantId,
            applicant?.FullName,
            applicant?.Email,
            app.CoverLetter,
            app.ExpectedSalary,
            app.ResumeId,
            app.AppliedVia,
            app.Status.ToString(),
            app.IsViewed,
            app.ViewedAt,
            app.CreatedOn);
    }
}
