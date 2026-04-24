namespace JobPortal.Application.Common.Models;

/// <summary>
/// Generic result type for Application layer operations.
/// Avoids exception-driven flow for expected business errors.
/// </summary>
public sealed class Result<T>
{
    public bool IsSuccess { get; private init; }
    public T? Value { get; private init; }
    public string? Error { get; private init; }
    public IReadOnlyList<ValidationError> ValidationErrors { get; private init; } = [];

    public bool IsFailure => !IsSuccess;

    private Result() { }

    public static Result<T> Success(T value) =>
        new() { IsSuccess = true, Value = value };

    public static Result<T> Failure(string error) =>
        new() { IsSuccess = false, Error = error };

    public static Result<T> ValidationFailure(IReadOnlyList<ValidationError> errors) =>
        new() { IsSuccess = false, ValidationErrors = errors };
}

public sealed class Result
{
    public bool IsSuccess { get; private init; }
    public string? Error { get; private init; }
    public IReadOnlyList<ValidationError> ValidationErrors { get; private init; } = [];

    public bool IsFailure => !IsSuccess;

    public static Result Success() => new() { IsSuccess = true };

    public static Result Failure(string error) =>
        new() { IsSuccess = false, Error = error };
}

public sealed record ValidationError(string Field, string Message);
