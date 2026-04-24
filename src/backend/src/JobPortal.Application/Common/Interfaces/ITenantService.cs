namespace JobPortal.Application.Common.Interfaces;

/// <summary>
/// Resolves the current tenant from the HTTP request context.
/// Implemented in Infrastructure via TenantMiddleware.
/// </summary>
public interface ITenantService
{
    Guid? TenantId { get; }
    string? TenantSlug { get; }
    bool IsResolved { get; }

    Guid GetRequiredTenantId();
}
