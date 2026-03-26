using ProjectManagement.Core.Entities;
namespace ProjectManagement.Core.Interfaces;
public interface ITeamRepository : IRepository<Team>
{
    Task<IEnumerable<Team>> GetByUserIdAsync(Guid userId, CancellationToken ct);
    Task<Team?> GetWithMembersAsync(Guid id, CancellationToken ct);
    Task<bool> HasActiveProjectsAsync(Guid teamId, CancellationToken ct);
}
