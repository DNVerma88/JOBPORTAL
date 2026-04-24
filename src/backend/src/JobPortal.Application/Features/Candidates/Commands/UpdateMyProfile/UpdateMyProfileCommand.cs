using FluentValidation;
using JobPortal.Application.Common.Interfaces;
using JobPortal.Domain.Entities.Portal;
using JobPortal.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobPortal.Application.Features.Candidates.Commands.UpdateMyProfile;

public sealed record UpdateMyProfileCommand(
    string? Headline,
    string? Summary,
    string? CurrentJobTitle,
    string? CurrentCompany,
    short? TotalExperienceYears,
    DateOnly? DateOfBirth,
    Gender? Gender,
    string? Nationality,
    Guid? CountryId,
    Guid? CityId,
    string? Address,
    ProfileVisibility ProfileVisibility,
    bool IsOpenToWork,
    decimal? ExpectedSalaryMin,
    decimal? ExpectedSalaryMax,
    string? PreferredCurrencyCode,
    short? NoticePeriodDays,
    string? LinkedInUrl,
    string? GitHubUrl,
    string? PortfolioUrl
) : IRequest<Guid>;

public sealed class UpdateMyProfileCommandHandler(
    IApplicationDbContext db,
    ICurrentUserService currentUser)
    : IRequestHandler<UpdateMyProfileCommand, Guid>
{
    public async Task<Guid> Handle(UpdateMyProfileCommand request, CancellationToken ct)
    {
        var userId = currentUser.UserId ?? Guid.Empty;
        var tenantId = currentUser.TenantId ?? Guid.Empty;

        var profile = await db.JobSeekerProfiles.FirstOrDefaultAsync(p => p.UserId == userId, ct);
        if (profile is null)
        {
            profile = JobSeekerProfile.Create(tenantId, userId, userId);
            db.JobSeekerProfiles.Add(profile);
        }

        profile.Update(request.Headline, request.Summary, request.CurrentJobTitle,
            request.CurrentCompany, request.TotalExperienceYears, request.DateOfBirth,
            request.Gender, request.Nationality, request.CountryId, request.CityId,
            request.Address, request.ProfileVisibility, request.IsOpenToWork,
            request.ExpectedSalaryMin, request.ExpectedSalaryMax, request.PreferredCurrencyCode,
            request.NoticePeriodDays, request.LinkedInUrl, request.GitHubUrl, request.PortfolioUrl, userId);

        await db.SaveChangesAsync(ct);
        return profile.Id;
    }
}
