using JobPortal.Domain.Common;
using JobPortal.Domain.Enums;

namespace JobPortal.Domain.Entities.Portal;

public sealed class Notification : BaseEntity
{
    public Guid UserId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Body { get; private set; } = string.Empty;
    public NotificationType Type { get; private set; }
    public NotificationDeliveryScope DeliveryScope { get; private set; } = NotificationDeliveryScope.Private;
    public NotificationChannel Channel { get; private set; } = NotificationChannel.InApp;
    public bool IsRead { get; private set; }
    public DateTimeOffset? ReadAt { get; private set; }
    public string? ActionUrl { get; private set; }
    public DateTimeOffset? ExpiresAt { get; private set; }

    private Notification() { }

    public static Notification Create(Guid tenantId, Guid userId, string title, string body,
        NotificationType type, NotificationChannel channel, string? actionUrl,
        DateTimeOffset? expiresAt, Guid createdBy) =>
        new()
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            UserId = userId,
            Title = title.Trim(),
            Body = body.Trim(),
            Type = type,
            Channel = channel,
            ActionUrl = actionUrl?.Trim(),
            ExpiresAt = expiresAt,
            CreatedBy = createdBy,
            CreatedOn = DateTimeOffset.UtcNow
        };

    public void MarkRead(Guid modifiedBy)
    {
        IsRead = true;
        ReadAt = DateTimeOffset.UtcNow;
        ModifiedBy = modifiedBy;
        ModifiedOn = DateTimeOffset.UtcNow;
    }
}
