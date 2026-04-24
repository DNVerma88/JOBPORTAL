using JobPortal.Domain.Common;

namespace JobPortal.Domain.Entities.Portal;

/// <summary>Tracks each stage movement of a candidate — used for Kanban board.</summary>
public sealed class CandidatePipeline : BaseEntity
{
    public Guid ApplicationId { get; private set; }
    public Guid StageId { get; private set; }
    public Guid MovedBy { get; private set; }
    public DateTimeOffset MovedAt { get; private set; }
    public string? Notes { get; private set; }

    private CandidatePipeline() { }

    public static CandidatePipeline Create(Guid tenantId, Guid applicationId, Guid stageId,
        Guid movedBy, string? notes, Guid createdBy) =>
        new()
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            ApplicationId = applicationId,
            StageId = stageId,
            MovedBy = movedBy,
            MovedAt = DateTimeOffset.UtcNow,
            Notes = notes?.Trim(),
            CreatedBy = createdBy,
            CreatedOn = DateTimeOffset.UtcNow
        };
}
