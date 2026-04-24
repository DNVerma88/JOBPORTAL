using MediatR;

namespace JobPortal.Domain.Common;

/// <summary>
/// Marker interface for all domain events.
/// Domain events are handled by MediatR INotificationHandler implementations.
/// </summary>
public interface IDomainEvent : INotification;
