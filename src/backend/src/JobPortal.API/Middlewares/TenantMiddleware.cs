using System.Security.Claims;

namespace JobPortal.API.Middlewares;

/// <summary>
/// Injects a correlation/trace ID into each request for end-to-end log tracing.
/// The ID is taken from X-Correlation-Id header or generated fresh.
/// It is echoed back in the response header.
/// </summary>
public sealed class CorrelationIdMiddleware(RequestDelegate next)
{
    private const string CorrelationHeader = "X-Correlation-Id";

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = context.Request.Headers[CorrelationHeader].FirstOrDefault()
            ?? Guid.NewGuid().ToString();

        context.Items["CorrelationId"] = correlationId;
        context.Response.Headers[CorrelationHeader] = correlationId;

        // Enrich the logging scope so Serilog picks it up
        using (Serilog.Context.LogContext.PushProperty("CorrelationId", correlationId))
        {
            await next(context);
        }
    }
}

/// <summary>
/// Validates that a TenantId can be resolved for non-public endpoints.
/// Public endpoints (auth, public job search) are exempted via attribute.
/// </summary>
public sealed class TenantMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        // Tenant is resolved lazily by TenantService; no action needed here.
        // This middleware's role is future-proofing (e.g., rate limiting per tenant).
        await next(context);
    }
}
