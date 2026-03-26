using ProjectManagement.Core.Entities;
namespace ProjectManagement.Core.Interfaces;
public interface IProjectRepository : IRepository<Project>
{
    Task<Project?> GetWithStatsAsync(Guid id, CancellationToken ct);
    Task<IEnumerable<Project>> GetByTeamAsync(Guid teamId, CancellationToken ct);
    Task<bool> IsKeyUniqueInTeamAsync(Guid teamId, string key, Guid? excludeProjectId, CancellationToken ct);
    Task<IEnumerable<Project>> GetAccessibleByUserAsync(Guid userId, string? teamId, string? status, int page, int pageSize, CancellationToken ct);
    Task<int> CountAccessibleByUserAsync(Guid userId, string? teamId, string? status, CancellationToken ct);
}
