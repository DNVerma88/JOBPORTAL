using JobPortal.Application.Common.Interfaces;
using JobPortal.Application.Features.Candidates.Queries.GetMyProfile;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobPortal.Application.Features.Candidates.Queries.GetCandidateById;

public sealed record GetCandidateByIdQuery(Guid ProfileId) : IRequest<CandidateProfileDto?>;

public sealed class GetCandidateByIdQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetCandidateByIdQuery, CandidateProfileDto?>
{
    public async Task<CandidateProfileDto?> Handle(GetCandidateByIdQuery request, CancellationToken ct)
    {
        var profile = await db.JobSeekerProfiles
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(p => p.Id == request.ProfileId, ct);

        if (profile is null) return null;

        return new CandidateProfileDto(
            profile.Id, profile.UserId, profile.Headline, profile.Summary,
            profile.CurrentJobTitle, profile.CurrentCompany, profile.TotalExperienceYears,
            profile.DateOfBirth, profile.Gender?.ToString(), profile.Nationality,
            profile.CountryId, profile.CityId, profile.Address,
            profile.ProfileVisibility.ToString(), profile.IsOpenToWork,
            profile.ExpectedSalaryMin, profile.ExpectedSalaryMax, profile.PreferredCurrencyCode,
            profile.NoticePeriodDays, profile.LinkedInUrl, profile.GitHubUrl,
            profile.PortfolioUrl, profile.ResumeUrl, profile.ProfileCompletionPct);
    }
}
