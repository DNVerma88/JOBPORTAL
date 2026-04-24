using JobPortal.Domain.Common;
using JobPortal.Domain.Enums;

namespace JobPortal.Domain.Entities.Portal;

public sealed class JobAlert : BaseEntity
{
    public Guid UserId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string[]? Keywords { get; private set; }
    public Guid[]? CategoryIds { get; private set; }
    public Guid[]? CityIds { get; private set; }
    public string[]? JobTypes { get; private set; }
    public string[]? WorkModes { get; private set; }
    public decimal? MinSalary { get; private set; }
    public decimal? MaxSalary { get; private set; }
    public string[]? ExperienceLevels { get; private set; }
    public AlertFrequency Frequency { get; private set; } = AlertFrequency.Daily;
    public bool IsActive { get; private set; } = true;
    public DateTimeOffset? LastSentAt { get; private set; }

    private JobAlert() { }

    public static JobAlert Create(Guid tenantId, Guid userId, string name, AlertFrequency frequency, Guid createdBy) =>
        new()
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            UserId = userId,
            Name = name.Trim(),
            Frequency = frequency,
            CreatedBy = createdBy,
            CreatedOn = DateTimeOffset.UtcNow
        };

    public void Toggle(Guid modifiedBy)
    {
        IsActive = !IsActive;
        ModifiedBy = modifiedBy;
        ModifiedOn = DateTimeOffset.UtcNow;
    }

    public void UpdateFilters(string[]? keywords, Guid[]? categoryIds, Guid[]? cityIds,
        string[]? jobTypes, string[]? workModes, decimal? minSalary, decimal? maxSalary,
        string[]? experienceLevels, AlertFrequency frequency, Guid modifiedBy)
    {
        Keywords = keywords;
        CategoryIds = categoryIds;
        CityIds = cityIds;
        JobTypes = jobTypes;
        WorkModes = workModes;
        MinSalary = minSalary;
        MaxSalary = maxSalary;
        ExperienceLevels = experienceLevels;
        Frequency = frequency;
        ModifiedBy = modifiedBy;
        ModifiedOn = DateTimeOffset.UtcNow;
    }
}
