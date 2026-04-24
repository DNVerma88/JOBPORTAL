using JobPortal.Domain.Common;

namespace JobPortal.Domain.Entities.Portal;

public sealed class Education : BaseEntity
{
    public Guid ProfileId { get; private set; }
    public string Degree { get; private set; } = string.Empty;
    public string FieldOfStudy { get; private set; } = string.Empty;
    public string Institution { get; private set; } = string.Empty;
    public Guid? EducationLevelId { get; private set; }
    public short? StartYear { get; private set; }
    public short? EndYear { get; private set; }
    public bool IsCurrent { get; private set; }
    public string? Grade { get; private set; }
    public string? Description { get; private set; }

    private Education() { }

    public static Education Create(
        Guid tenantId, Guid profileId,
        string degree, string fieldOfStudy, string institution,
        short? startYear, short? endYear, bool isCurrent,
        string? grade, string? description,
        Guid? educationLevelId,
        Guid createdBy) =>
        new()
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            ProfileId = profileId,
            Degree = degree.Trim(),
            FieldOfStudy = fieldOfStudy.Trim(),
            Institution = institution.Trim(),
            StartYear = startYear,
            EndYear = isCurrent ? null : endYear,
            IsCurrent = isCurrent,
            Grade = grade?.Trim(),
            Description = description?.Trim(),
            EducationLevelId = educationLevelId,
            CreatedBy = createdBy,
            CreatedOn = DateTimeOffset.UtcNow
        };
}
