using JobPortal.Domain.Common;

namespace JobPortal.Domain.Entities.Portal;

public sealed class CandidatePipelineStage : BaseEntity
{
    public Guid JobPostingId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? Color { get; private set; }
    public short SortOrder { get; private set; }
    public bool IsDefault { get; private set; }

    private CandidatePipelineStage() { }

    public static CandidatePipelineStage Create(Guid tenantId, Guid jobPostingId, string name,
        string? color, short sortOrder, bool isDefault, Guid createdBy) =>
        new()
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            JobPostingId = jobPostingId,
            Name = name.Trim(),
            Color = color?.Trim(),
            SortOrder = sortOrder,
            IsDefault = isDefault,
            CreatedBy = createdBy,
            CreatedOn = DateTimeOffset.UtcNow
        };

    public void Update(string name, string? color, short sortOrder, Guid modifiedBy)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Name = name.Trim();
        Color = color?.Trim();
        SortOrder = sortOrder;
        ModifiedBy = modifiedBy;
        ModifiedOn = DateTimeOffset.UtcNow;
    }
}
