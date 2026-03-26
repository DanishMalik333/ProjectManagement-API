using Microsoft.EntityFrameworkCore;
using ProjectManagement.Core.Entities;
using ProjectManagement.Core.Enums;
using ProjectManagement.Core.Interfaces;
using ProjectManagement.Infrastructure.Persistence;

namespace ProjectManagement.Infrastructure.Repositories;

public class SprintRepository : GenericRepository<Sprint>, ISprintRepository
{
    public SprintRepository(AppDbContext context) : base(context) { }

    public async Task<Sprint?> GetActiveSprintAsync(Guid projectId, CancellationToken ct = default) =>
        await _context.Sprints
            .FirstOrDefaultAsync(s => s.ProjectId == projectId && s.Status == SprintStatus.Active, ct);

    public async Task<IEnumerable<Sprint>> GetWithTaskCountAsync(Guid projectId, CancellationToken ct = default) =>
        await _context.Sprints
            .Include(s => s.Tasks)
            .Where(s => s.ProjectId == projectId)
            .ToListAsync(ct);

    public async Task<IEnumerable<Sprint>> GetByProjectAsync(Guid projectId, string? status, CancellationToken ct = default)
    {
        var query = _context.Sprints
            .Include(s => s.Tasks)
            .Where(s => s.ProjectId == projectId);

        if (!string.IsNullOrEmpty(status) && Enum.TryParse<SprintStatus>(status, true, out var s))
            query = query.Where(sp => sp.Status == s);

        return await query.ToListAsync(ct);
    }
}
