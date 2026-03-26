using Microsoft.EntityFrameworkCore;
using ProjectManagement.Core.Entities;
using ProjectManagement.Core.Enums;
using ProjectManagement.Core.Interfaces;
using ProjectManagement.Infrastructure.Persistence;

namespace ProjectManagement.Infrastructure.Repositories;

public class ProjectRepository : GenericRepository<Project>, IProjectRepository
{
    public ProjectRepository(AppDbContext context) : base(context) { }

    public async Task<Project?> GetWithStatsAsync(Guid id, CancellationToken ct = default) =>
        await _context.Projects
            .Include(p => p.Team)
            .Include(p => p.Owner)
            .Include(p => p.Sprints)
            .Include(p => p.Tasks)
            .FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task<IEnumerable<Project>> GetByTeamAsync(Guid teamId, CancellationToken ct = default) =>
        await _context.Projects
            .Include(p => p.Team)
            .Include(p => p.Owner)
            .Where(p => p.TeamId == teamId)
            .ToListAsync(ct);

    public async Task<bool> IsKeyUniqueInTeamAsync(Guid teamId, string key, Guid? excludeProjectId, CancellationToken ct = default) =>
        !await _context.Projects
            .IgnoreQueryFilters()
            .AnyAsync(p => p.TeamId == teamId && p.Key == key && p.Id != (excludeProjectId ?? Guid.Empty), ct);

    public async Task<IEnumerable<Project>> GetAccessibleByUserAsync(Guid userId, string? teamId, string? status, int page, int pageSize, CancellationToken ct = default)
    {
        var userTeamIds = await _context.TeamMembers
            .Where(tm => tm.UserId == userId)
            .Select(tm => tm.TeamId)
            .ToListAsync(ct);

        var query = _context.Projects
            .Include(p => p.Team)
            .Include(p => p.Owner)
            .Where(p => userTeamIds.Contains(p.TeamId));

        if (!string.IsNullOrEmpty(teamId) && Guid.TryParse(teamId, out var tid))
            query = query.Where(p => p.TeamId == tid);

        if (!string.IsNullOrEmpty(status) && Enum.TryParse<ProjectStatus>(status, true, out var s))
            query = query.Where(p => p.Status == s);

        return await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
    }

    public async Task<int> CountAccessibleByUserAsync(Guid userId, string? teamId, string? status, CancellationToken ct = default)
    {
        var userTeamIds = await _context.TeamMembers
            .Where(tm => tm.UserId == userId)
            .Select(tm => tm.TeamId)
            .ToListAsync(ct);

        var query = _context.Projects.Where(p => userTeamIds.Contains(p.TeamId));

        if (!string.IsNullOrEmpty(teamId) && Guid.TryParse(teamId, out var tid))
            query = query.Where(p => p.TeamId == tid);

        if (!string.IsNullOrEmpty(status) && Enum.TryParse<ProjectStatus>(status, true, out var s))
            query = query.Where(p => p.Status == s);

        return await query.CountAsync(ct);
    }
}
