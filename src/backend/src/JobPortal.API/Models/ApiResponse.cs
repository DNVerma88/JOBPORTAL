namespace JobPortal.API.Models;

/// <summary>
/// Standardize all API responses in a consistent envelope.
/// </summary>
public sealed record ApiResponse<T>(
    bool Success,
    T? Data,
    string? Message,
    IReadOnlyList<ApiError>? Errors,
    string? TraceId,
    DateTimeOffset Timestamp)
{
    public static ApiResponse<T> Ok(T data, string? message = null, string? traceId = null) =>
        new(true, data, message, null, traceId, DateTimeOffset.UtcNow);

    public static ApiResponse<T> Fail(string message, IReadOnlyList<ApiError>? errors = null, string? traceId = null) =>
        new(false, default, message, errors, traceId, DateTimeOffset.UtcNow);
}

public sealed record ApiResponse(
    bool Success,
    string? Message,
    IReadOnlyList<ApiError>? Errors,
    string? TraceId,
    DateTimeOffset Timestamp)
{
    public static ApiResponse Ok(string? message = null, string? traceId = null) =>
        new(true, message, null, traceId, DateTimeOffset.UtcNow);

    public static ApiResponse Fail(string message, IReadOnlyList<ApiError>? errors = null, string? traceId = null) =>
        new(false, message, errors, traceId, DateTimeOffset.UtcNow);
}

public sealed record ApiError(string Field, string Message);
