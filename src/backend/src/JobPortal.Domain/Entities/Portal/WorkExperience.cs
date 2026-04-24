using JobPortal.Domain.Common;

namespace JobPortal.Domain.Entities.Portal;

public sealed class WorkExperience : BaseEntity
{
    public Guid ProfileId { get; private set; }
    public string JobTitle { get; private set; } = string.Empty;
    public string CompanyName { get; private set; } = string.Empty;
    public Guid? CompanyId { get; private set; }
    public Guid? IndustryId { get; private set; }
    public Guid? CityId { get; private set; }
    public Guid? CountryId { get; private set; }
    public DateOnly StartDate { get; private set; }
    public DateOnly? EndDate { get; private set; }
    public bool IsCurrent { get; private set; }
    public string? Description { get; private set; }
    public string[]? Skills { get; private set; }

    private WorkExperience() { }

    public static WorkExperience Create(
        Guid tenantId, Guid profileId,
        string jobTitle, string companyName,
        DateOnly startDate, DateOnly? endDate, bool isCurrent,
        string? description, string[]? skills,
        Guid? cityId, Guid? countryId, Guid? industryId,
        Guid createdBy) =>
        new()
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            ProfileId = profileId,
            JobTitle = jobTitle.Trim(),
            CompanyName = companyName.Trim(),
            StartDate = startDate,
            EndDate = isCurrent ? null : endDate,
            IsCurrent = isCurrent,
            Description = description?.Trim(),
            Skills = skills,
            CityId = cityId,
            CountryId = countryId,
            IndustryId = industryId,
            CreatedBy = createdBy,
            CreatedOn = DateTimeOffset.UtcNow
        };
}
