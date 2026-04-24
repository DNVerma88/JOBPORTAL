using JobPortal.API.Models;
using JobPortal.Application.Common.Models;
using JobPortal.Application.Features.Notifications.NotificationFeatures;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobPortal.API.Controllers.v1;

[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
[Authorize]
public sealed class NotificationsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] bool? unreadOnly = null,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetNotificationsQuery(pageNumber, pageSize, unreadOnly), ct);
        return Ok(ApiResponse<PagedList<NotificationDto>>.Ok(result, "Notifications retrieved."));
    }

    [HttpPatch("{id:guid}/read")]
    public async Task<IActionResult> MarkRead(Guid id, CancellationToken ct)
    {
        await mediator.Send(new MarkNotificationReadCommand(id), ct);
        return Ok(ApiResponse.Ok("Notification marked as read."));
    }

    [HttpPost("read-all")]
    public async Task<IActionResult> MarkAllRead(CancellationToken ct)
    {
        await mediator.Send(new MarkAllNotificationsReadCommand(), ct);
        return Ok(ApiResponse.Ok("All notifications marked as read."));
    }

    [HttpGet("unread-count")]
    public async Task<IActionResult> UnreadCount(CancellationToken ct)
    {
        var count = await mediator.Send(new GetUnreadCountQuery(), ct);
        return Ok(ApiResponse<object>.Ok(new { count }, "Unread count retrieved."));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await mediator.Send(new DeleteNotificationCommand(id), ct);
        return Ok(ApiResponse.Ok("Notification deleted."));
    }
}
