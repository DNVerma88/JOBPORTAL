using JobPortal.Domain.Common;

namespace JobPortal.Domain.Events.Auth;

public sealed record UserRegisteredEvent(Guid UserId, Guid TenantId, string Email) : IDomainEvent;

public sealed record UserPasswordChangedEvent(Guid UserId, Guid TenantId) : IDomainEvent;

public sealed record UserEmailVerifiedEvent(Guid UserId, Guid TenantId) : IDomainEvent;

public sealed record UserLockedOutEvent(Guid UserId, Guid TenantId, DateTimeOffset LockedUntil) : IDomainEvent;
