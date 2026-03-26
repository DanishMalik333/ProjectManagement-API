using ProjectManagement.Core.Entities;
namespace ProjectManagement.Core.Interfaces;
public interface INotificationRepository : IRepository<Notification>
{
    Task<IEnumerable<Notification>> GetPagedByUserAsync(Guid userId, bool? isRead, int page, int pageSize, CancellationToken ct);
    Task<int> CountByUserAsync(Guid userId, bool? isRead, CancellationToken ct);
    Task<int> GetUnreadCountAsync(Guid userId, CancellationToken ct);
    Task MarkAllReadAsync(Guid userId, CancellationToken ct);
}
