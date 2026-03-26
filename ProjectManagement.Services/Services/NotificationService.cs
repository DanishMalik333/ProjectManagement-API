using ProjectManagement.Core.DTOs.Common;
using ProjectManagement.Core.DTOs.Notifications;
using ProjectManagement.Core.Entities;
using ProjectManagement.Core.Enums;
using ProjectManagement.Core.Exceptions;
using ProjectManagement.Core.Interfaces;

namespace ProjectManagement.Services.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public NotificationService(INotificationRepository notificationRepository, IUnitOfWork unitOfWork)
    {
        _notificationRepository = notificationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task CreateAsync(Guid userId, NotificationType type, string message, Guid referenceId, string referenceType, CancellationToken ct = default)
    {
        var notification = new Notification { UserId = userId, Type = type, Message = message, ReferenceId = referenceId, ReferenceType = referenceType, IsRead = false };
        await _notificationRepository.AddAsync(notification, ct);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task<PagedResult<NotificationResponse>> GetPagedAsync(Guid userId, bool? isRead, int page, int pageSize, CancellationToken ct = default)
    {
        var notifications = await _notificationRepository.GetAllAsync(ct);
        var filtered = notifications.Where(n => n.UserId == userId);
        if (isRead.HasValue) filtered = filtered.Where(n => n.IsRead == isRead);
        var totalCount = filtered.Count();
        var paged = filtered.OrderByDescending(n => n.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize).ToList();
        var responses = paged.Select(n => new NotificationResponse(n.Id, n.UserId, n.Type.ToString(), n.Message, n.ReferenceId, n.ReferenceType, n.IsRead, n.CreatedAt)).ToList();
        var totalPages = (totalCount + pageSize - 1) / pageSize;
        return new PagedResult<NotificationResponse>(responses, totalCount, page, pageSize, totalPages);
    }

    public async Task MarkReadAsync(Guid notificationId, Guid userId, CancellationToken ct = default)
    {
        var notification = await _notificationRepository.GetByIdAsync(notificationId, ct);
        if (notification == null) throw new NotFoundException("Notification not found.");
        if (notification.UserId != userId) throw new ForbiddenException("Not your notification.");
        notification.IsRead = true;
        _notificationRepository.Update(notification);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task MarkAllReadAsync(Guid userId, CancellationToken ct = default)
    {
        var notifications = await _notificationRepository.GetAllAsync(ct);
        foreach (var n in notifications.Where(x => x.UserId == userId && !x.IsRead))
        {
            n.IsRead = true;
            _notificationRepository.Update(n);
        }
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task<UnreadCountResponse> GetUnreadCountAsync(Guid userId, CancellationToken ct = default)
    {
        var notifications = await _notificationRepository.GetAllAsync(ct);
        var count = notifications.Count(n => n.UserId == userId && !n.IsRead);
        return new UnreadCountResponse(count);
    }
}
