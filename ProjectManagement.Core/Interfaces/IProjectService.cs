using ProjectManagement.Core.DTOs.Common;
using ProjectManagement.Core.DTOs.Projects;
namespace ProjectManagement.Core.Interfaces;
public interface IProjectService
{
    Task<PagedResult<ProjectResponse>> GetAccessibleAsync(Guid userId, string? teamId, string? status, int page, int pageSize, CancellationToken ct = default);
    Task<ProjectResponse> CreateAsync(CreateProjectRequest request, Guid creatorId, CancellationToken ct = default);
    Task<ProjectResponse> GetByIdAsync(Guid id, Guid currentUserId, CancellationToken ct = default);
    Task<ProjectResponse> UpdateAsync(Guid id, UpdateProjectRequest request, Guid currentUserId, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
    Task<ProjectStatsResponse> GetStatsAsync(Guid id, Guid currentUserId, CancellationToken ct = default);
    Task<IEnumerable<LabelResponse>> GetLabelsAsync(Guid projectId, Guid currentUserId, CancellationToken ct = default);
    Task<LabelResponse> CreateLabelAsync(Guid projectId, CreateLabelRequest request, CancellationToken ct = default);
    Task<LabelResponse> UpdateLabelAsync(Guid projectId, Guid labelId, UpdateLabelRequest request, CancellationToken ct = default);
    Task DeleteLabelAsync(Guid projectId, Guid labelId, CancellationToken ct = default);
}
