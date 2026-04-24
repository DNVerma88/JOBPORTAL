namespace JobPortal.Application.Common.Interfaces;

/// <summary>
/// Provides current authenticated user context anywhere in the Application layer.
/// Implemented in Infrastructure via IHttpContextAccessor.
/// </summary>
public interface ICurrentUserService
{
    Guid? UserId { get; }
    Guid? TenantId { get; }
    string? Email { get; }
    IReadOnlyList<string> Roles { get; }
    IReadOnlyList<string> Permissions { get; }
    bool IsAuthenticated { get; }
    bool IsInRole(string role);
    bool HasPermission(string permission);

    /// <summary>Throws UnauthorizedAccessException if not authenticated.</summary>
    Guid GetRequiredUserId();

    /// <summary>Throws UnauthorizedAccessException if TenantId is missing.</summary>
    Guid GetRequiredTenantId();
}
