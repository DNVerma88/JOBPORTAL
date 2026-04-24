using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace JobPortal.Infrastructure.Hubs;

/// <summary>
/// Maps the JWT 'sub' claim to the SignalR user identifier.
/// This enables Clients.User(userId) to target the correct connection.
/// </summary>
public sealed class NotificationUserIdProvider : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection) =>
        connection.User?.FindFirstValue(ClaimTypes.NameIdentifier);
}
