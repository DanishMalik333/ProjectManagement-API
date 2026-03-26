using Microsoft.EntityFrameworkCore;
using ProjectManagement.Core.Entities;
using ProjectManagement.Core.Interfaces;
using ProjectManagement.Infrastructure.Persistence;

namespace ProjectManagement.Infrastructure.Repositories;

public class TeamRepository : GenericRepository<Team>, ITeamRepository
{
    public TeamRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Team>> GetByUserIdAsync(Guid userId, CancellationToken ct = default) =>
        await _context.Teams
            .Include(t => t.TeamMembers).ThenInclude(tm => tm.User)
            .Include(t => t.CreatedBy)
            .Where(t => t.TeamMembers.Any(tm => tm.UserId == userId))
            .ToListAsync(ct);

    public async Task<Team?> GetWithMembersAsync(Guid id, CancellationToken ct = default) =>
        await _context.Teams
            .Include(t => t.CreatedBy)
            .Include(t => t.TeamMembers).ThenInclude(tm => tm.User)
            .FirstOrDefaultAsync(t => t.Id == id, ct);

    public async Task<bool> HasActiveProjectsAsync(Guid teamId, CancellationToken ct = default) =>
        await _context.Projects
            .IgnoreQueryFilters()
            .AnyAsync(p => p.TeamId == teamId && p.Status == Core.Enums.ProjectStatus.Active && !p.IsDeleted, ct);
}
