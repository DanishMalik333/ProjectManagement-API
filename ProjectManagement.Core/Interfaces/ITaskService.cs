using ProjectManagement.Core.DTOs.Tasks;
using ProjectManagement.Core.DTOs.Common;
namespace ProjectManagement.Core.Interfaces;
public interface ITaskService
{
    Task<PagedResult<TaskResponse>> GetFilteredAsync(TaskFilterParams filter, CancellationToken ct = default);
    Task<TaskResponse> CreateAsync(CreateTaskRequest request, Guid reporterId, CancellationToken ct = default);
    Task<TaskResponse> GetByIdAsync(Guid id, Guid currentUserId, CancellationToken ct = default);
    Task<TaskResponse> UpdateAsync(Guid id, UpdateTaskRequest request, Guid currentUserId, CancellationToken ct = default);
    Task DeleteAsync(Guid id, Guid currentUserId, CancellationToken ct = default);
    Task<TaskResponse> UpdateStatusAsync(Guid id, UpdateTaskStatusRequest request, Guid currentUserId, CancellationToken ct = default);
    Task<TaskResponse> UpdateAssigneeAsync(Guid id, UpdateAssigneeRequest request, Guid currentUserId, CancellationToken ct = default);
    Task<TaskResponse> MoveToSprintAsync(Guid id, UpdateSprintRequest request, Guid currentUserId, CancellationToken ct = default);
    Task AddLabelAsync(Guid id, AddLabelToTaskRequest request, Guid currentUserId, CancellationToken ct = default);
    Task RemoveLabelAsync(Guid id, Guid labelId, Guid currentUserId, CancellationToken ct = default);
    Task<IEnumerable<TaskHistoryResponse>> GetHistoryAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<TaskResponse>> GetSubtasksAsync(Guid id, CancellationToken ct = default);
    Task<TaskResponse> ReorderAsync(Guid id, ReorderTaskRequest request, Guid currentUserId, CancellationToken ct = default);
}
