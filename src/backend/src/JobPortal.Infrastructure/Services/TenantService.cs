using JobPortal.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace JobPortal.Infrastructure.Services;

/// <summary>
/// Resolves tenant from JWT claim or X-Tenant-Id header.
/// Populated by TenantMiddleware before any handler runs.
/// </summary>
public sealed class TenantService(IHttpContextAccessor httpContextAccessor) : ITenantService
{
    private const string TenantHeader = "X-Tenant-Id";
    private const string TenantClaim = "tenant_id";

    public Guid? TenantId
    {
        get
        {
            var httpContext = httpContextAccessor.HttpContext;
            if (httpContext is null) return null;

            // 1. JWT claim (authenticated requests)
            var claim = httpContext.User.FindFirst(TenantClaim)?.Value;
            if (Guid.TryParse(claim, out var id)) return id;

            // 2. Header (unauthenticated public requests or service-to-service)
            var header = httpContext.Request.Headers[TenantHeader].FirstOrDefault();
            if (Guid.TryParse(header, out var headerId)) return headerId;

            return null;
        }
    }

    public string? TenantSlug =>
        httpContextAccessor.HttpContext?.Request.Host.Host.Split('.').FirstOrDefault();

    public bool IsResolved => TenantId.HasValue;

    public Guid GetRequiredTenantId() =>
        TenantId ?? throw new InvalidOperationException("Tenant could not be resolved from request.");
}
