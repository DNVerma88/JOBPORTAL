using JobPortal.Application.Common.Interfaces;
using JobPortal.Application.Common.Models;
using JobPortal.Domain.Entities.Portal;
using JobPortal.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobPortal.Application.Features.Notifications.NotificationFeatures;

// ── Get Notifications ─────────────────────────────────────────────────────────
public sealed record GetNotificationsQuery(int PageNumber = 1, int PageSize = 20, bool? UnreadOnly = null)
    : IRequest<PagedList<NotificationDto>>;

public sealed record NotificationDto(
    Guid Id, string Title, string Body, string Type, bool IsRead, DateTimeOffset? ReadAt,
    string? ActionUrl, DateTimeOffset CreatedOn);

public sealed class GetNotificationsQueryHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    : IRequestHandler<GetNotificationsQuery, PagedList<NotificationDto>>
{
    public async Task<PagedList<NotificationDto>> Handle(GetNotificationsQuery request, CancellationToken ct)
    {
        var userId = currentUser.UserId ?? Guid.Empty;
        var query = db.Notifications.Where(n => n.UserId == userId);

        if (request.UnreadOnly == true)
            query = query.Where(n => !n.IsRead);

        var total = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(n => n.CreatedOn)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(n => new NotificationDto(n.Id, n.Title, n.Body, n.Type.ToString(),
                n.IsRead, n.ReadAt, n.ActionUrl, n.CreatedOn))
            .ToListAsync(ct);

        return PagedList<NotificationDto>.Create(items, total, request.PageNumber, request.PageSize);
    }
}

// ── Mark Notification Read ────────────────────────────────────────────────────
public sealed record MarkNotificationReadCommand(Guid Id) : IRequest;

public sealed class MarkNotificationReadCommandHandler(
    IApplicationDbContext db,
    ICurrentUserService currentUser)
    : IRequestHandler<MarkNotificationReadCommand>
{
    public async Task Handle(MarkNotificationReadCommand request, CancellationToken ct)
    {
        var userId = currentUser.UserId ?? Guid.Empty;
        var notification = await db.Notifications.FirstOrDefaultAsync(n => n.Id == request.Id, ct)
            ?? throw new KeyNotFoundException($"Notification {request.Id} not found.");
        notification.MarkRead(userId);
        await db.SaveChangesAsync(ct);
    }
}

// ── Mark All Read ─────────────────────────────────────────────────────────────
public sealed record MarkAllNotificationsReadCommand : IRequest;

public sealed class MarkAllNotificationsReadCommandHandler(
    IApplicationDbContext db,
    ICurrentUserService currentUser)
    : IRequestHandler<MarkAllNotificationsReadCommand>
{
    public async Task Handle(MarkAllNotificationsReadCommand request, CancellationToken ct)
    {
        var userId = currentUser.UserId ?? Guid.Empty;
        var unread = await db.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .ToListAsync(ct);

        foreach (var n in unread)
            n.MarkRead(userId);

        await db.SaveChangesAsync(ct);
    }
}

// ── Get Unread Count ────────────────────────────────────────────────────────────
public sealed record GetUnreadCountQuery : IRequest<int>;

public sealed class GetUnreadCountQueryHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    : IRequestHandler<GetUnreadCountQuery, int>
{
    public async Task<int> Handle(GetUnreadCountQuery request, CancellationToken ct)
    {
        var userId = currentUser.UserId ?? Guid.Empty;
        return await db.Notifications.CountAsync(n => n.UserId == userId && !n.IsRead, ct);
    }
}

// ── Delete Notification ─────────────────────────────────────────────────────────
public sealed record DeleteNotificationCommand(Guid Id) : IRequest;

public sealed class DeleteNotificationCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    : IRequestHandler<DeleteNotificationCommand>
{
    public async Task Handle(DeleteNotificationCommand request, CancellationToken ct)
    {
        var userId = currentUser.UserId ?? Guid.Empty;
        var notification = await db.Notifications.FirstOrDefaultAsync(n => n.Id == request.Id, ct)
            ?? throw new KeyNotFoundException($"Notification {request.Id} not found.");
        db.Notifications.Remove(notification);
        await db.SaveChangesAsync(ct);
    }
}
