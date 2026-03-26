using ProjectManagement.Core.DTOs.Tasks;
using ProjectManagement.Core.Entities;
using ProjectManagement.Core.Exceptions;
using ProjectManagement.Core.Interfaces;

namespace ProjectManagement.Services.Services;

public class TaskHistoryService : ITaskHistoryService
{
    private readonly ITaskHistoryRepository _taskHistoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public TaskHistoryService(ITaskHistoryRepository taskHistoryRepository, IUnitOfWork unitOfWork)
    {
        _taskHistoryRepository = taskHistoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task RecordAsync(Guid taskId, Guid userId, string fieldName, string? oldValue, string? newValue, CancellationToken ct = default)
    {
        var history = new TaskHistory { TaskId = taskId, ChangedByUserId = userId, FieldName = fieldName, OldValue = oldValue, NewValue = newValue };
        await _taskHistoryRepository.AddAsync(history, ct);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task<IEnumerable<TaskHistoryResponse>> GetByTaskAsync(Guid taskId, CancellationToken ct = default)
    {
        var histories = await _taskHistoryRepository.GetAllAsync(ct);
        return histories.Where(h => h.TaskId == taskId).OrderByDescending(h => h.ChangedAt).Select(h => new TaskHistoryResponse(h.Id, h.TaskId, h.ChangedByUserId, "User", h.FieldName, h.OldValue, h.NewValue, h.ChangedAt));
    }
}
