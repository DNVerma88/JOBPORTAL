using JobPortal.Domain.Common;

namespace JobPortal.Domain.Entities.Auth;

/// <summary>
/// SHA-256 hash of the refresh token is stored — plaintext is only returned once to the client.
/// </summary>
public sealed class RefreshToken : BaseEntity
{
    public Guid UserId { get; private set; }
    public string TokenHash { get; private set; } = string.Empty; // SHA-256 hash
    public DateTimeOffset ExpiresOn { get; private set; }
    public bool IsRevoked { get; private set; }
    public DateTimeOffset? RevokedOn { get; private set; }
    public string? RevokedReason { get; private set; }
    public string? DeviceInfo { get; private set; }
    public string? IpAddress { get; private set; }

    private RefreshToken() { }

    public static RefreshToken Create(
        Guid tenantId,
        Guid userId,
        string tokenHash,
        DateTimeOffset expiresOn,
        string? deviceInfo,
        string? ipAddress,
        Guid createdBy) =>
        new()
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            UserId = userId,
            TokenHash = tokenHash,
            ExpiresOn = expiresOn,
            DeviceInfo = deviceInfo,
            IpAddress = ipAddress,
            CreatedBy = createdBy,
            CreatedOn = DateTimeOffset.UtcNow
        };

    public bool IsExpired() => DateTimeOffset.UtcNow >= ExpiresOn;

    public bool IsActive() => !IsRevoked && !IsExpired();

    public void Revoke(string reason, Guid revokedBy)
    {
        IsRevoked = true;
        RevokedOn = DateTimeOffset.UtcNow;
        RevokedReason = reason;
        ModifiedBy = revokedBy;
        ModifiedOn = DateTimeOffset.UtcNow;
    }
}
