using JobPortal.Domain.Common;
using JobPortal.Domain.Enums;

namespace JobPortal.Domain.Entities.Portal;

public sealed class JobSeekerProfile : BaseEntity, IAggregateRoot
{
    public Guid UserId { get; private set; }
    public string? Headline { get; private set; }
    public string? Summary { get; private set; }
    public string? CurrentJobTitle { get; private set; }
    public string? CurrentCompany { get; private set; }
    public short? TotalExperienceYears { get; private set; }
    public DateOnly? DateOfBirth { get; private set; }
    public Gender? Gender { get; private set; }
    public string? Nationality { get; private set; }
    public Guid? CountryId { get; private set; }
    public Guid? CityId { get; private set; }
    public string? Address { get; private set; }
    public ProfileVisibility ProfileVisibility { get; private set; } = ProfileVisibility.Public;
    public bool IsOpenToWork { get; private set; } = true;
    public decimal? ExpectedSalaryMin { get; private set; }
    public decimal? ExpectedSalaryMax { get; private set; }
    public string? PreferredCurrencyCode { get; private set; }
    public short? NoticePeriodDays { get; private set; }
    public string? LinkedInUrl { get; private set; }
    public string? GitHubUrl { get; private set; }
    public string? PortfolioUrl { get; private set; }
    public string? ResumeUrl { get; private set; }
    public short ProfileCompletionPct { get; private set; }
    public bool IsVerified { get; private set; }
    public DateTimeOffset? LastActiveAt { get; private set; }

    private JobSeekerProfile() { }

    public static JobSeekerProfile Create(Guid tenantId, Guid userId, Guid createdBy) =>
        new()
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            UserId = userId,
            CreatedBy = createdBy,
            CreatedOn = DateTimeOffset.UtcNow
        };

    public void Update(string? headline, string? summary, string? currentJobTitle, string? currentCompany,
        short? totalExperience, DateOnly? dob, Enums.Gender? gender, string? nationality,
        Guid? countryId, Guid? cityId, string? address, ProfileVisibility visibility,
        bool isOpenToWork, decimal? salaryMin, decimal? salaryMax, string? currencyCode,
        short? noticeDays, string? linkedIn, string? gitHub, string? portfolio, Guid modifiedBy)
    {
        Headline = headline?.Trim();
        Summary = summary?.Trim();
        CurrentJobTitle = currentJobTitle?.Trim();
        CurrentCompany = currentCompany?.Trim();
        TotalExperienceYears = totalExperience;
        DateOfBirth = dob;
        Gender = gender;
        Nationality = nationality?.Trim();
        CountryId = countryId;
        CityId = cityId;
        Address = address?.Trim();
        ProfileVisibility = visibility;
        IsOpenToWork = isOpenToWork;
        ExpectedSalaryMin = salaryMin;
        ExpectedSalaryMax = salaryMax;
        PreferredCurrencyCode = currencyCode;
        NoticePeriodDays = noticeDays;
        LinkedInUrl = linkedIn?.Trim();
        GitHubUrl = gitHub?.Trim();
        PortfolioUrl = portfolio?.Trim();
        ModifiedBy = modifiedBy;
        ModifiedOn = DateTimeOffset.UtcNow;
    }

    public void UpdateResumeUrl(string? resumeUrl, Guid modifiedBy)
    {
        ResumeUrl = resumeUrl;
        ModifiedBy = modifiedBy;
        ModifiedOn = DateTimeOffset.UtcNow;
    }
}
