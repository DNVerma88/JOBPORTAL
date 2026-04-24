using JobPortal.Domain.Enums;

namespace JobPortal.Application.Common.Interfaces;

public record NotificationPayload(
    string Title,
    string Body,
    NotificationType Type,
    NotificationDeliveryScope Scope,
    Dictionary<string, string>? Data = null,
    Guid? TargetUserId = null,
    Guid? TargetTenantId = null,
    string? TargetRoleKey = null);

public interface INotificationService
{
    /// <summary>Sends to all connected users.</summary>
    Task SendPublicAsync(NotificationPayload payload, CancellationToken cancellationToken = default);

    /// <summary>Sends to all users of a specific tenant.</summary>
    Task SendToTenantAsync(Guid tenantId, NotificationPayload payload, CancellationToken cancellationToken = default);

    /// <summary>Sends to a single user (private).</summary>
    Task SendToUserAsync(Guid userId, NotificationPayload payload, CancellationToken cancellationToken = default);

    /// <summary>Sends to all members of a role within a tenant.</summary>
    Task SendToRoleAsync(Guid tenantId, string role, NotificationPayload payload, CancellationToken cancellationToken = default);
}
