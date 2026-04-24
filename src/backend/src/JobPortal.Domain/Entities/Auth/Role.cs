using JobPortal.Domain.Common;

namespace JobPortal.Domain.Entities.Auth;

public sealed class Role : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string NormalizedName { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public bool IsSystemRole { get; private set; } // cannot be deleted

    private Role() { }

    public static Role Create(Guid tenantId, string name, string? description, bool isSystemRole, Guid createdBy)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        return new Role
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Name = name.Trim(),
            NormalizedName = name.Trim().ToUpperInvariant(),
            Description = description?.Trim(),
            IsSystemRole = isSystemRole,
            CreatedBy = createdBy,
            CreatedOn = DateTimeOffset.UtcNow
        };
    }

    public void Update(string name, string? description, Guid modifiedBy)
    {
        Name = name.Trim();
        NormalizedName = name.Trim().ToUpperInvariant();
        Description = description?.Trim();
        ModifiedBy = modifiedBy;
        ModifiedOn = DateTimeOffset.UtcNow;
    }
}
