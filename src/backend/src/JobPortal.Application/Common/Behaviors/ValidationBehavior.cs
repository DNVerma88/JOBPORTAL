using FluentValidation;
using JobPortal.Application.Common.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace JobPortal.Application.Common.Behaviors;

/// <summary>
/// MediatR pipeline behavior that runs FluentValidation validators before the handler.
/// Collects all errors rather than short-circuiting on first failure.
/// </summary>
public sealed class ValidationBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators,
    ILogger<ValidationBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!validators.Any())
            return await next(cancellationToken);

        var context = new ValidationContext<TRequest>(request);
        var validationResults = await Task.WhenAll(
            validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f is not null)
            .ToList();

        if (failures.Count == 0)
            return await next(cancellationToken);

        logger.LogWarning("Validation failed for {RequestType}: {Errors}",
            typeof(TRequest).Name,
            string.Join(", ", failures.Select(f => $"{f.PropertyName}: {f.ErrorMessage}")));

        var errors = failures
            .Select(f => new ValidationError(f.PropertyName, f.ErrorMessage))
            .ToList();

        // Result<T> pattern — if TResponse is Result<T>, construct it; otherwise throw
        var responseType = typeof(TResponse);
        if (responseType.IsGenericType && responseType.GetGenericTypeDefinition() == typeof(Result<>))
        {
            var createMethod = responseType.GetMethod(nameof(Result<object>.ValidationFailure))!;
            return (TResponse)createMethod.Invoke(null, [errors])!;
        }

        if (responseType == typeof(Result))
            return (TResponse)(object)Result.Failure(string.Join("; ", failures.Select(f => f.ErrorMessage)));

        throw new ValidationException(failures);
    }
}
