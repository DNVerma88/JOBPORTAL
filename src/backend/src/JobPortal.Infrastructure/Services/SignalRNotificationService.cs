using JobPortal.Application.Common.Interfaces;
using JobPortal.Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace JobPortal.Infrastructure.Services;

public sealed class SignalRNotificationService(
    IHubContext<NotificationHub, INotificationHubClient> hubContext) : INotificationService
{
    public async Task SendPublicAsync(NotificationPayload payload, CancellationToken cancellationToken = default)
    {
        var message = Map(payload);
        await hubContext.Clients.All.ReceiveNotification(message);
    }

    public async Task SendToTenantAsync(Guid tenantId, NotificationPayload payload, CancellationToken cancellationToken = default)
    {
        var message = Map(payload);
        await hubContext.Clients.Group($"tenant:{tenantId}").ReceiveNotification(message);
    }

    public async Task SendToUserAsync(Guid userId, NotificationPayload payload, CancellationToken cancellationToken = default)
    {
        var message = Map(payload);
        // Clients.User uses IUserIdProvider (maps JWT sub)
        await hubContext.Clients.User(userId.ToString()).ReceiveNotification(message);
    }

    public async Task SendToRoleAsync(Guid tenantId, string role, NotificationPayload payload, CancellationToken cancellationToken = default)
    {
        var message = Map(payload);
        await hubContext.Clients.Group($"role:{tenantId}:{role}").ReceiveNotification(message);
    }

    private static NotificationMessage Map(NotificationPayload payload) =>
        new(
            Guid.NewGuid(),
            payload.Title,
            payload.Body,
            payload.Type.ToString(),
            payload.Data,
            DateTimeOffset.UtcNow);
}
