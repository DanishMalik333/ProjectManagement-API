using ProjectManagement.Core.Entities;
namespace ProjectManagement.Core.Interfaces;
public interface ILabelRepository : IRepository<Label>
{
    Task<IEnumerable<Label>> GetByProjectAsync(Guid projectId, CancellationToken ct);
    Task<Label?> GetByIdAndProjectAsync(Guid id, Guid projectId, CancellationToken ct);
}
