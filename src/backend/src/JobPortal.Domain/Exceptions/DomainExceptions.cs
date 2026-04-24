namespace JobPortal.Domain.Exceptions;

public abstract class DomainException(string message) : Exception(message);

public sealed class EntityNotFoundException(string entityName, object key)
    : DomainException($"Entity '{entityName}' with key '{key}' was not found.");

public sealed class DuplicateEntityException(string entityName, string field)
    : DomainException($"A '{entityName}' with the same '{field}' already exists.");

public sealed class BusinessRuleViolationException(string message)
    : DomainException(message);

public sealed class ConcurrencyException(string entityName)
    : DomainException($"A concurrency conflict occurred on entity '{entityName}'. The record was modified by another user.");

public sealed class UnauthorizedDomainException(string message)
    : DomainException(message);

public sealed class TenantMismatchException()
    : DomainException("Access denied: resource belongs to a different tenant.");

public sealed class SubscriptionLimitExceededException(string limitName)
    : DomainException($"Subscription limit exceeded: '{limitName}'. Please upgrade your plan.");
