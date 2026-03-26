using ProjectManagement.Core.DTOs.Sprints;
namespace ProjectManagement.Core.Interfaces;
public interface ISprintService
{
    Task<IEnumerable<SprintResponse>> GetByProjectAsync(Guid projectId, string? status, Guid currentUserId, CancellationToken ct = default);
    Task<SprintResponse> CreateAsync(Guid projectId, CreateSprintRequest request, Guid currentUserId, CancellationToken ct = default);
    Task<SprintResponse> GetByIdAsync(Guid projectId, Guid sprintId, Guid currentUserId, CancellationToken ct = default);
    Task<SprintResponse> UpdateAsync(Guid projectId, Guid sprintId, UpdateSprintRequest request, Guid currentUserId, CancellationToken ct = default);
    Task<SprintResponse> StartAsync(Guid projectId, Guid sprintId, Guid currentUserId, CancellationToken ct = default);
    Task<SprintResponse> CompleteAsync(Guid projectId, Guid sprintId, CompleteSprintRequest request, Guid currentUserId, CancellationToken ct = default);
    Task DeleteAsync(Guid projectId, Guid sprintId, Guid currentUserId, CancellationToken ct = default);
}
