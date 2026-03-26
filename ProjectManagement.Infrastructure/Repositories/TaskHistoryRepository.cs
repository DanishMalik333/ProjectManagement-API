using Microsoft.EntityFrameworkCore;
using ProjectManagement.Core.Entities;
using ProjectManagement.Core.Interfaces;
using ProjectManagement.Infrastructure.Persistence;

namespace ProjectManagement.Infrastructure.Repositories;

public class TaskHistoryRepository : GenericRepository<TaskHistory>, ITaskHistoryRepository
{
    public TaskHistoryRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<TaskHistory>> GetByTaskAsync(Guid taskId, CancellationToken ct = default) =>
        await _context.TaskHistories
            .Include(h => h.ChangedBy)
            .Where(h => h.TaskId == taskId)
            .OrderByDescending(h => h.ChangedAt)
            .ToListAsync(ct);
}
