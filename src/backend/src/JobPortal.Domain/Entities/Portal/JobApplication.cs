using JobPortal.Domain.Common;
using JobPortal.Domain.Enums;

namespace JobPortal.Domain.Entities.Portal;

public sealed class JobApplication : BaseEntity, IAggregateRoot
{
    public Guid JobPostingId { get; private set; }
    public Guid ApplicantId { get; private set; }
    public Guid? ProfileId { get; private set; }
    public Guid? ResumeId { get; private set; }
    public string? CoverLetter { get; private set; }
    public decimal? ExpectedSalary { get; private set; }
    public ApplicationStatus Status { get; private set; } = ApplicationStatus.Pending;
    public string? AppliedVia { get; private set; }
    public string? ReferralCode { get; private set; }
    public bool IsViewed { get; private set; }
    public DateTimeOffset? ViewedAt { get; private set; }
    public string? Source { get; private set; }

    private JobApplication() { }

    public static JobApplication Create(Guid tenantId, Guid jobPostingId, Guid applicantId,
        Guid? profileId, Guid? resumeId, string? coverLetter, decimal? expectedSalary,
        string? appliedVia, Guid createdBy) =>
        new()
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            JobPostingId = jobPostingId,
            ApplicantId = applicantId,
            ProfileId = profileId,
            ResumeId = resumeId,
            CoverLetter = coverLetter?.Trim(),
            ExpectedSalary = expectedSalary,
            AppliedVia = appliedVia,
            CreatedBy = createdBy,
            CreatedOn = DateTimeOffset.UtcNow
        };

    public void UpdateStatus(ApplicationStatus status, Guid modifiedBy)
    {
        Status = status;
        ModifiedBy = modifiedBy;
        ModifiedOn = DateTimeOffset.UtcNow;
    }

    public void MarkViewed(Guid modifiedBy)
    {
        IsViewed = true;
        ViewedAt = DateTimeOffset.UtcNow;
        ModifiedBy = modifiedBy;
        ModifiedOn = DateTimeOffset.UtcNow;
    }

    public void Withdraw(Guid modifiedBy)
    {
        Status = ApplicationStatus.Withdrawn;
        ModifiedBy = modifiedBy;
        ModifiedOn = DateTimeOffset.UtcNow;
    }
}
