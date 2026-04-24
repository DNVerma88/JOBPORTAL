using FluentValidation;
using JobPortal.Application.Common.Interfaces;
using JobPortal.Domain.Entities.Portal;
using JobPortal.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobPortal.Application.Features.Candidates.Queries.GetMyProfile;

public sealed record GetMyProfileQuery : IRequest<CandidateProfileDto?>;

public sealed record CandidateProfileDto(
    Guid Id,
    Guid UserId,
    string? Headline,
    string? Summary,
    string? CurrentJobTitle,
    string? CurrentCompany,
    short? TotalExperienceYears,
    DateOnly? DateOfBirth,
    string? Gender,
    string? Nationality,
    Guid? CountryId,
    Guid? CityId,
    string? Address,
    string ProfileVisibility,
    bool IsOpenToWork,
    decimal? ExpectedSalaryMin,
    decimal? ExpectedSalaryMax,
    string? PreferredCurrencyCode,
    short? NoticePeriodDays,
    string? LinkedInUrl,
    string? GitHubUrl,
    string? PortfolioUrl,
    string? ResumeUrl,
    short ProfileCompletionPct
);

public sealed class GetMyProfileQueryHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    : IRequestHandler<GetMyProfileQuery, CandidateProfileDto?>
{
    public async Task<CandidateProfileDto?> Handle(GetMyProfileQuery request, CancellationToken ct)
    {
        var userId = currentUser.UserId ?? Guid.Empty;
        var profile = await db.JobSeekerProfiles.FirstOrDefaultAsync(p => p.UserId == userId, ct);
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
