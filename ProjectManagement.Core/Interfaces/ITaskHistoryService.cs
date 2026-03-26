using ProjectManagement.Core.DTOs.Tasks;
namespace ProjectManagement.Core.Interfaces;
public interface ITaskHistoryService
{
    Task RecordAsync(Guid taskId, Guid userId, string fieldName, string? oldValue, string? newValue, CancellationToken ct = default);
    Task<IEnumerable<TaskHistoryResponse>> GetByTaskAsync(Guid taskId, CancellationToken ct = default);
}
