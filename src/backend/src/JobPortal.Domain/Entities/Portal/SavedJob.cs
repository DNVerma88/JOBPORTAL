using JobPortal.Domain.Common;

namespace JobPortal.Domain.Entities.Portal;

/// <summary>SavedJobs is a lightweight bookmark — no TenantId isolation needed beyond the TenantId FK.</summary>
public sealed class SavedJob : BaseEntity
{
    public Guid UserId { get; private set; }
    public Guid JobPostingId { get; private set; }

    private SavedJob() { }

    public static SavedJob Create(Guid tenantId, Guid userId, Guid jobPostingId, Guid createdBy) =>
        new()
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            UserId = userId,
            JobPostingId = jobPostingId,
            CreatedBy = createdBy,
            CreatedOn = DateTimeOffset.UtcNow
        };
}
