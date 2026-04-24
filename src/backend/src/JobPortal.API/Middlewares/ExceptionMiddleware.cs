using JobPortal.API.Models;
using JobPortal.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace JobPortal.API.Middlewares;

/// <summary>
/// Global exception handler middleware.
/// Catches all unhandled exceptions and returns structured ApiResponse envelopes.
/// Detailed errors only shown in Development to prevent information leakage (OWASP A05).
/// </summary>
public sealed class ExceptionMiddleware(
    RequestDelegate next,
    ILogger<ExceptionMiddleware> logger,
    IHostEnvironment env)
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var traceId = context.TraceIdentifier;
        var (statusCode, message, errors) = MapException(exception, env.IsDevelopment());

        logger.LogError(exception,
            "Unhandled exception. TraceId: {TraceId}, Path: {Path}, StatusCode: {StatusCode}",
            traceId, context.Request.Path, statusCode);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var response = ApiResponse.Fail(message, errors, traceId);
        var json = JsonSerializer.Serialize(response, JsonOptions);

        await context.Response.WriteAsync(json);
    }

    private static (HttpStatusCode StatusCode, string Message, IReadOnlyList<ApiError>? Errors) MapException(
        Exception exception, bool isDevelopment)
    {
        return exception switch
        {
            EntityNotFoundException ex =>
                (HttpStatusCode.NotFound, ex.Message, null),

            DuplicateEntityException ex =>
                (HttpStatusCode.Conflict, ex.Message, null),

            BusinessRuleViolationException ex =>
                (HttpStatusCode.UnprocessableEntity, ex.Message, null),

            ConcurrencyException ex =>
                (HttpStatusCode.Conflict, ex.Message, null),

            UnauthorizedDomainException ex =>
                (HttpStatusCode.Unauthorized, ex.Message, null),

            TenantMismatchException ex =>
                (HttpStatusCode.Forbidden, ex.Message, null),

            SubscriptionLimitExceededException ex =>
                (HttpStatusCode.PaymentRequired, ex.Message, null),

            UnauthorizedAccessException ex =>
                (HttpStatusCode.Unauthorized, ex.Message, null),

            FluentValidation.ValidationException ex =>
                (HttpStatusCode.BadRequest, "Validation failed.",
                    ex.Errors.Select(e => new ApiError(e.PropertyName, e.ErrorMessage)).ToList()),

            OperationCanceledException =>
                (HttpStatusCode.BadRequest, "The request was cancelled.", null),

            _ => (HttpStatusCode.InternalServerError,
                isDevelopment ? exception.Message : "An unexpected error occurred. Please try again later.",
                null)
        };
    }
}
