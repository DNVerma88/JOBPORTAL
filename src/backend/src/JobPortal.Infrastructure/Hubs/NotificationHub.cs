using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace JobPortal.Infrastructure.Hubs;

/// <summary>
/// Strongly-typed SignalR hub contract for notification clients.
/// </summary>
public interface INotificationHubClient
{
    Task ReceiveNotification(NotificationMessage message);
    Task NotificationCountUpdated(int unreadCount);
}

public sealed record NotificationMessage(
    Guid NotificationId,
    string Title,
    string Body,
    string Type,
    Dictionary<string, string>? Data,
    DateTimeOffset SentAt);

/// <summary>
/// SignalR notification hub.
/// - Users are automatically added to personal group (user:{id}) and tenant group (tenant:{id}).
/// - Public notifications use Clients.All.
/// - Private notifications use Clients.User(userId) — mapped by IUserIdProvider.
/// - Tenant-scoped: Clients.Group("tenant:{tenantId}").
/// - Role-scoped:   Clients.Group("role:{tenantId}:{roleName}").
/// </summary>
[Authorize]
public sealed class NotificationHub(ILogger<NotificationHub> logger) 
    : Hub<INotificationHubClient>
{
    public override async Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier;
        var tenantId = Context.User?.FindFirst("tenant_id")?.Value;

        if (!string.IsNullOrEmpty(userId))
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user:{userId}");

        if (!string.IsNullOrEmpty(tenantId))
            await Groups.AddToGroupAsync(Context.ConnectionId, $"tenant:{tenantId}");

        // Add role groups
        var roles = Context.User?.FindAll(System.Security.Claims.ClaimTypes.Role)
            .Select(c => c.Value) ?? [];

        foreach (var role in roles)
        {
            if (!string.IsNullOrEmpty(tenantId))
                await Groups.AddToGroupAsync(Context.ConnectionId, $"role:{tenantId}:{role}");
        }

        logger.LogInformation("User {UserId} connected to NotificationHub (TenantId: {TenantId})",
            userId, tenantId);

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.UserIdentifier;
        logger.LogInformation("User {UserId} disconnected from NotificationHub", userId);
        await base.OnDisconnectedAsync(exception);
    }

    public async Task MarkAsRead(Guid notificationId)
    {
        // Clients call this hub method to mark a notification read
        // Real implementation dispatches a command via MediatR (injected via DI)
        logger.LogDebug("User {UserId} marked notification {NotificationId} as read",
            Context.UserIdentifier, notificationId);
        await Task.CompletedTask;
    }
}
