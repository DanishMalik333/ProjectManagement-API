using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagement.Core.DTOs.Notifications;
using ProjectManagement.Core.Interfaces;

namespace ProjectManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificationsController(INotificationService notificationService, ICurrentUserService currentUserService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<NotificationResponse>>> GetPaged(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] bool? isRead = null,
        CancellationToken ct = default)
    {
        var userId = currentUserService.UserId;
        var notifications = await notificationService.GetPagedAsync(userId, isRead, page, pageSize, ct);
        return Ok(notifications);
    }

    [HttpGet("unread-count")]
    public async Task<ActionResult<int>> GetUnreadCount(CancellationToken ct = default)
    {
        var userId = currentUserService.UserId;
        var count = await notificationService.GetUnreadCountAsync(userId, ct);
        return Ok(count);
    }

    [HttpPut("{id}/read")]
    public async Task<IActionResult> MarkRead(Guid id, CancellationToken ct = default)
    {
        var userId = currentUserService.UserId;
        await notificationService.MarkReadAsync(id, userId, ct);
        return NoContent();
    }

    [HttpPut("mark-all-read")]
    public async Task<IActionResult> MarkAllRead(CancellationToken ct = default)
    {
        var userId = currentUserService.UserId;
        await notificationService.MarkAllReadAsync(userId, ct);
        return NoContent();
    }


}
