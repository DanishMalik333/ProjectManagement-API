using ProjectManagement.Core.Entities;
namespace ProjectManagement.Core.Interfaces;
public interface ISprintRepository : IRepository<Sprint>
{
    Task<Sprint?> GetActiveSprintAsync(Guid projectId, CancellationToken ct);
    Task<IEnumerable<Sprint>> GetWithTaskCountAsync(Guid projectId, CancellationToken ct);
    Task<IEnumerable<Sprint>> GetByProjectAsync(Guid projectId, string? status, CancellationToken ct);
}
