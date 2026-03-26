using ProjectManagement.Core.DTOs.Common;
using ProjectManagement.Core.DTOs.Tasks;
using ProjectManagement.Core.Entities;
using ProjectManagement.Core.Enums;
using TaskStatus = ProjectManagement.Core.Enums.TaskStatus;
namespace ProjectManagement.Core.Interfaces;
public interface ITaskRepository : IRepository<ProjectTask>
{
    Task<PagedResult<ProjectTask>> GetFilteredAsync(TaskFilterParams filter, CancellationToken ct);
    Task<ProjectTask?> GetWithDetailsAsync(Guid id, CancellationToken ct);
    Task<IEnumerable<ProjectTask>> GetSubtasksAsync(Guid parentId, CancellationToken ct);
    Task<int> CountByStatusAsync(Guid projectId, TaskStatus status, CancellationToken ct);
    Task<IEnumerable<ProjectTask>> GetBySprintAsync(Guid sprintId, CancellationToken ct);
}
