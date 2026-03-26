using Microsoft.EntityFrameworkCore;
using ProjectManagement.Core.Entities;
using ProjectManagement.Core.Interfaces;
using ProjectManagement.Infrastructure.Persistence;

namespace ProjectManagement.Infrastructure.Repositories;

public class NotificationRepository : GenericRepository<Notification>, INotificationRepository
{
    public NotificationRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Notification>> GetPagedByUserAsync(Guid userId, bool? isRead, int page, int pageSize, CancellationToken ct = default)
    {
        var query = _context.Notifications.Where(n => n.UserId == userId);
        if (isRead.HasValue) query = query.Where(n => n.IsRead == isRead.Value);
        return await query.OrderByDescending(n => n.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
    }

    public async Task<int> CountByUserAsync(Guid userId, bool? isRead, CancellationToken ct = default)
    {
        var query = _context.Notifications.Where(n => n.UserId == userId);
        if (isRead.HasValue) query = query.Where(n => n.IsRead == isRead.Value);
        return await query.CountAsync(ct);
    }

    public async Task<int> GetUnreadCountAsync(Guid userId, CancellationToken ct = default) =>
        await _context.Notifications.CountAsync(n => n.UserId == userId && !n.IsRead, ct);

    public async Task MarkAllReadAsync(Guid userId, CancellationToken ct = default) =>
        await _context.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .ExecuteUpdateAsync(s => s.SetProperty(n => n.IsRead, true), ct);
}
