using JobPortal.Domain.Common;

namespace JobPortal.Domain.Entities.Auth;

public sealed class UserSession : BaseEntity
{
    public Guid UserId { get; private set; }
    public string SessionToken { get; private set; } = string.Empty;
    public string? DeviceType { get; private set; }
    public string? DeviceInfo { get; private set; }
    public string? IpAddress { get; private set; }
    public string? UserAgent { get; private set; }
    public bool IsActive { get; private set; } = true;
    public DateTimeOffset LastActivityAt { get; private set; }
    public DateTimeOffset ExpiresAt { get; private set; }

    private UserSession() { }

    public static UserSession Create(Guid tenantId, Guid userId, string sessionToken,
        string? deviceType, string? deviceInfo, string? ipAddress, string? userAgent,
        DateTimeOffset expiresAt, Guid createdBy) =>
        new()
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            UserId = userId,
            SessionToken = sessionToken,
            DeviceType = deviceType,
            DeviceInfo = deviceInfo,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            ExpiresAt = expiresAt,
            LastActivityAt = DateTimeOffset.UtcNow,
            CreatedBy = createdBy,
            CreatedOn = DateTimeOffset.UtcNow
        };

    public void Deactivate(Guid modifiedBy)
    {
        IsActive = false;
        ModifiedBy = modifiedBy;
        ModifiedOn = DateTimeOffset.UtcNow;
    }
}
