using ProjectManagement.Core.Entities;
namespace ProjectManagement.Core.Interfaces;
public interface ITaskHistoryRepository : IRepository<TaskHistory>
{
    Task<IEnumerable<TaskHistory>> GetByTaskAsync(Guid taskId, CancellationToken ct);
}
