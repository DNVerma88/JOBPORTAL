using JobPortal.Domain.Common;

namespace JobPortal.Domain.Entities.Auth;

public sealed class Permission : BaseEntity
{
    public string Name { get; private set; } = string.Empty; // e.g. "Jobs.Create"
    public string Resource { get; private set; } = string.Empty; // e.g. "Jobs"
    public string Action { get; private set; } = string.Empty; // e.g. "Create"
    public string? Description { get; private set; }

    private Permission() { }

    public static Permission Create(Guid tenantId, string name, string resource, string action, string? description, Guid createdBy)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(resource);
        ArgumentException.ThrowIfNullOrWhiteSpace(action);

        return new Permission
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Name = name.Trim(),
            Resource = resource.Trim(),
            Action = action.Trim(),
            Description = description?.Trim(),
            CreatedBy = createdBy,
            CreatedOn = DateTimeOffset.UtcNow
        };
    }
}

public sealed class UserRole : BaseEntity
{
    public Guid UserId { get; private set; }
    public Guid RoleId { get; private set; }
    public User? User { get; private set; }
    public Role? Role { get; private set; }

    private UserRole() { }

    public static UserRole Create(Guid tenantId, Guid userId, Guid roleId, Guid createdBy) =>
        new()
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            UserId = userId,
            RoleId = roleId,
            CreatedBy = createdBy,
            CreatedOn = DateTimeOffset.UtcNow
        };
}

public sealed class RolePermission : BaseEntity
{
    public Guid RoleId { get; private set; }
    public Guid PermissionId { get; private set; }
    public Role? Role { get; private set; }
    public Permission? Permission { get; private set; }

    private RolePermission() { }

    public static RolePermission Create(Guid tenantId, Guid roleId, Guid permissionId, Guid createdBy) =>
        new()
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            RoleId = roleId,
            PermissionId = permissionId,
            CreatedBy = createdBy,
            CreatedOn = DateTimeOffset.UtcNow
        };
}
