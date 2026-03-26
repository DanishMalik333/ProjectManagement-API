using ProjectManagement.Core.DTOs.Notifications;
using ProjectManagement.Core.DTOs.Common;
using ProjectManagement.Core.Enums;
namespace ProjectManagement.Core.Interfaces;
public interface INotificationService
{
    Task CreateAsync(Guid userId, NotificationType type, string message, Guid referenceId, string referenceType, CancellationToken ct = default);
    Task<PagedResult<NotificationResponse>> GetPagedAsync(Guid userId, bool? isRead, int page, int pageSize, CancellationToken ct = default);
    Task MarkReadAsync(Guid notificationId, Guid userId, CancellationToken ct = default);
    Task MarkAllReadAsync(Guid userId, CancellationToken ct = default);
    Task<UnreadCountResponse> GetUnreadCountAsync(Guid userId, CancellationToken ct = default);
}
