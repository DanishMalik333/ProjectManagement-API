using ProjectManagement.Core.Constants;
using ProjectManagement.Core.DTOs.Common;
using ProjectManagement.Core.DTOs.Tasks;
using ProjectManagement.Core.DTOs.Projects;
using ProjectManagement.Core.Entities;
using ProjectManagement.Core.Enums;
using ProjectManagement.Core.Exceptions;
using ProjectManagement.Core.Interfaces;
using TaskStatus = ProjectManagement.Core.Enums.TaskStatus;

namespace ProjectManagement.Services.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;
    private readonly ITaskHistoryRepository _taskHistoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public TaskService(ITaskRepository taskRepository, ITaskHistoryRepository taskHistoryRepository, IUnitOfWork unitOfWork)
    {
        _taskRepository = taskRepository;
        _taskHistoryRepository = taskHistoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResult<TaskResponse>> GetFilteredAsync(TaskFilterParams filter, CancellationToken ct = default)
    {
        var tasks = await _taskRepository.GetAllAsync(ct);
        var filtered = tasks.AsEnumerable();
        
        if (filter.ProjectId != Guid.Empty) 
            filtered = filtered.Where(t => t.ProjectId == filter.ProjectId);
        if (!string.IsNullOrEmpty(filter.SprintId) && Guid.TryParse(filter.SprintId, out var sprintId))
            filtered = filtered.Where(t => t.SprintId == sprintId);
        if (filter.AssigneeId.HasValue) 
            filtered = filtered.Where(t => t.AssigneeId == filter.AssigneeId);
        if (filter.Status?.Length > 0)
            filtered = filtered.Where(t => filter.Status.Contains(t.Status.ToString()));
        if (filter.Priority?.Length > 0)
            filtered = filtered.Where(t => filter.Priority.Contains(t.Priority.ToString()));
        if (filter.Type?.Length > 0)
            filtered = filtered.Where(t => filter.Type.Contains(t.Type.ToString()));
        if (!string.IsNullOrEmpty(filter.Search))
            filtered = filtered.Where(t => t.Title.Contains(filter.Search) || t.Description?.Contains(filter.Search) == true);
        if (filter.ParentTaskId.HasValue)
            filtered = filtered.Where(t => t.ParentTaskId == filter.ParentTaskId);
        
        // Apply sorting
        filtered = filter.SortDirection?.ToLower() == "desc"
            ? filtered.OrderByDescending(t => filter.SortBy?.ToLower() == "createdat" ? (object)t.CreatedAt : 
                                              filter.SortBy?.ToLower() == "priority" ? t.Priority :
                                              filter.SortBy?.ToLower() == "status" ? (object)t.Status :
                                              (object)t.OrderIndex)
            : filtered.OrderBy(t => filter.SortBy?.ToLower() == "createdat" ? (object)t.CreatedAt :
                                    filter.SortBy?.ToLower() == "priority" ? (object)t.Priority :
                                    filter.SortBy?.ToLower() == "status" ? (object)t.Status :
                                    (object)t.OrderIndex);
        
        var totalCount = filtered.Count();
        var paged = filtered.Skip((filter.Page - 1) * filter.PageSize).Take(filter.PageSize).ToList();
        var totalPages = (totalCount + filter.PageSize - 1) / filter.PageSize;
        
        var responses = paged.Select(t => new TaskResponse(
            t.Id, t.Title, t.Description,
            t.ProjectId, "Project",
            t.SprintId, "Sprint",
            t.AssigneeId, "Assignee",
            t.ReporterId, "Reporter",
            t.Status.ToString(), t.Priority.ToString(), t.Type.ToString(),
            t.StoryPoints, t.DueDate,
            t.ParentTaskId,
            t.OrderIndex,
            t.Comments.Count, t.Attachments.Count, t.Subtasks.Count,
            new List<LabelResponse>(),
            t.CreatedAt, t.UpdatedAt
        )).ToList();
        return new PagedResult<TaskResponse>(responses, totalCount, filter.Page, filter.PageSize, totalPages);
    }

    public async Task<TaskResponse> CreateAsync(CreateTaskRequest request, Guid reporterId, CancellationToken ct = default)
    {
        var taskType = !string.IsNullOrEmpty(request.Type) && Enum.TryParse<TaskType>(request.Type, ignoreCase: true, out var parsedType) 
            ? parsedType 
            : TaskType.Task;
        var priority = !string.IsNullOrEmpty(request.Priority) && Enum.TryParse<TaskPriority>(request.Priority, ignoreCase: true, out var parsedPriority)
            ? parsedPriority
            : TaskPriority.Medium;
            
        var task = new ProjectTask 
        { 
            ProjectId = request.ProjectId, 
            Title = request.Title, 
            Description = request.Description, 
            Type = taskType, 
            Priority = priority, 
            Status = TaskStatus.Todo, 
            StoryPoints = request.StoryPoints, 
            ParentTaskId = request.ParentTaskId, 
            AssigneeId = request.AssigneeId, 
            ReporterId = reporterId,
            DueDate = request.DueDate
        };
        await _taskRepository.AddAsync(task, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return new TaskResponse(
            task.Id, task.Title, task.Description,
            task.ProjectId, "Project",
            task.SprintId, "Sprint",
            task.AssigneeId, "Assignee",
            task.ReporterId, "Reporter",
            task.Status.ToString(), task.Priority.ToString(), task.Type.ToString(),
            task.StoryPoints, task.DueDate,
            task.ParentTaskId,
            task.OrderIndex,
            task.Comments.Count, task.Attachments.Count, task.Subtasks.Count,
            new List<LabelResponse>(),
            task.CreatedAt, task.UpdatedAt
        );
    }

    public async Task<TaskResponse> GetByIdAsync(Guid id, Guid currentUserId, CancellationToken ct = default)
    {
        var task = await _taskRepository.GetByIdAsync(id, ct);
        if (task == null) throw new NotFoundException("Task not found.");
        return new TaskResponse(
            task.Id, task.Title, task.Description,
            task.ProjectId, "Project",
            task.SprintId, "Sprint",
            task.AssigneeId, "Assignee",
            task.ReporterId, "Reporter",
            task.Status.ToString(), task.Priority.ToString(), task.Type.ToString(),
            task.StoryPoints, task.DueDate,
            task.ParentTaskId,
            task.OrderIndex,
            task.Comments.Count, task.Attachments.Count, task.Subtasks.Count,
            new List<LabelResponse>(),
            task.CreatedAt, task.UpdatedAt
        );
    }

    public async Task<TaskResponse> UpdateAsync(Guid id, UpdateTaskRequest request, Guid currentUserId, CancellationToken ct = default)
    {
        var task = await _taskRepository.GetByIdAsync(id, ct);
        if (task == null) throw new NotFoundException("Task not found.");
        if (!string.IsNullOrEmpty(request.Title)) task.Title = request.Title;
        if (!string.IsNullOrEmpty(request.Description)) task.Description = request.Description;
        if (!string.IsNullOrEmpty(request.Priority) && Enum.TryParse<TaskPriority>(request.Priority, ignoreCase: true, out var priority))
            task.Priority = priority;
        if (!string.IsNullOrEmpty(request.Type) && Enum.TryParse<TaskType>(request.Type, ignoreCase: true, out var taskType))
            task.Type = taskType;
        if (request.StoryPoints.HasValue) task.StoryPoints = request.StoryPoints;
        if (request.DueDate.HasValue) task.DueDate = request.DueDate;
        _taskRepository.Update(task);
        await _unitOfWork.SaveChangesAsync(ct);
        return new TaskResponse(
            task.Id, task.Title, task.Description,
            task.ProjectId, "Project",
            task.SprintId, "Sprint",
            task.AssigneeId, "Assignee",
            task.ReporterId, "Reporter",
            task.Status.ToString(), task.Priority.ToString(), task.Type.ToString(),
            task.StoryPoints, task.DueDate,
            task.ParentTaskId,
            task.OrderIndex,
            task.Comments.Count, task.Attachments.Count, task.Subtasks.Count,
            new List<LabelResponse>(),
            task.CreatedAt, task.UpdatedAt
        );
    }

    public async Task DeleteAsync(Guid id, Guid currentUserId, CancellationToken ct = default)
    {
        var task = await _taskRepository.GetByIdAsync(id, ct);
        if (task == null) throw new NotFoundException("Task not found.");
        task.IsDeleted = true;
        _taskRepository.Update(task);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task<TaskResponse> UpdateStatusAsync(Guid id, UpdateTaskStatusRequest request, Guid currentUserId, CancellationToken ct = default)
    {
        var task = await _taskRepository.GetByIdAsync(id, ct);
        if (task == null) throw new NotFoundException("Task not found.");
        
        if (!Enum.TryParse<TaskStatus>(request.Status, ignoreCase: true, out var newStatus))
            throw new BusinessRuleViolationException($"Invalid task status '{request.Status}'.");
        
        if (!TaskTransitions.IsAllowed(task.Status, newStatus))
            throw new BusinessRuleViolationException($"Invalid status transition from {task.Status} to {newStatus}.");
        
        task.Status = newStatus;
        _taskRepository.Update(task);
        await _unitOfWork.SaveChangesAsync(ct);
        return new TaskResponse(
            task.Id, task.Title, task.Description,
            task.ProjectId, "Project",
            task.SprintId, "Sprint",
            task.AssigneeId, "Assignee",
            task.ReporterId, "Reporter",
            task.Status.ToString(), task.Priority.ToString(), task.Type.ToString(),
            task.StoryPoints, task.DueDate,
            task.ParentTaskId,
            task.OrderIndex,
            task.Comments.Count, task.Attachments.Count, task.Subtasks.Count,
            new List<LabelResponse>(),
            task.CreatedAt, task.UpdatedAt
        );
    }

    public async Task<TaskResponse> UpdateAssigneeAsync(Guid id, UpdateAssigneeRequest request, Guid currentUserId, CancellationToken ct = default)
    {
        var task = await _taskRepository.GetByIdAsync(id, ct);
        if (task == null) throw new NotFoundException("Task not found.");
        task.AssigneeId = request.AssigneeId;
        _taskRepository.Update(task);
        await _unitOfWork.SaveChangesAsync(ct);
        return new TaskResponse(
            task.Id, task.Title, task.Description,
            task.ProjectId, "Project",
            task.SprintId, "Sprint",
            task.AssigneeId, "Assignee",
            task.ReporterId, "Reporter",
            task.Status.ToString(), task.Priority.ToString(), task.Type.ToString(),
            task.StoryPoints, task.DueDate,
            task.ParentTaskId,
            task.OrderIndex,
            task.Comments.Count, task.Attachments.Count, task.Subtasks.Count,
            new List<LabelResponse>(),
            task.CreatedAt, task.UpdatedAt
        );
    }

    public async Task<TaskResponse> MoveToSprintAsync(Guid id, UpdateSprintRequest request, Guid currentUserId, CancellationToken ct = default)
    {
        var task = await _taskRepository.GetByIdAsync(id, ct);
        if (task == null) throw new NotFoundException("Task not found.");
        task.SprintId = request.SprintId;
        _taskRepository.Update(task);
        await _unitOfWork.SaveChangesAsync(ct);
        return new TaskResponse(
            task.Id, task.Title, task.Description,
            task.ProjectId, "Project",
            task.SprintId, "Sprint",
            task.AssigneeId, "Assignee",
            task.ReporterId, "Reporter",
            task.Status.ToString(), task.Priority.ToString(), task.Type.ToString(),
            task.StoryPoints, task.DueDate,
            task.ParentTaskId,
            task.OrderIndex,
            task.Comments.Count, task.Attachments.Count, task.Subtasks.Count,
            new List<LabelResponse>(),
            task.CreatedAt, task.UpdatedAt
        );
    }

    public async Task AddLabelAsync(Guid id, AddLabelToTaskRequest request, Guid currentUserId, CancellationToken ct = default)
    {
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task RemoveLabelAsync(Guid id, Guid labelId, Guid currentUserId, CancellationToken ct = default)
    {
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task<IEnumerable<TaskHistoryResponse>> GetHistoryAsync(Guid id, CancellationToken ct = default)
    {
        var histories = await _taskHistoryRepository.GetAllAsync(ct);
        return histories.Where(h => h.TaskId == id).Select(h => new TaskHistoryResponse(h.Id, h.TaskId, h.ChangedByUserId, "User", h.FieldName, h.OldValue, h.NewValue, h.ChangedAt));
    }

    public async Task<IEnumerable<TaskResponse>> GetSubtasksAsync(Guid id, CancellationToken ct = default)
    {
        var tasks = await _taskRepository.GetAllAsync(ct);
        return tasks.Where(t => t.ParentTaskId == id && !t.IsDeleted).Select(t => new TaskResponse(
            t.Id, t.Title, t.Description,
            t.ProjectId, "Project",
            t.SprintId, "Sprint",
            t.AssigneeId, "Assignee",
            t.ReporterId, "Reporter",
            t.Status.ToString(), t.Priority.ToString(), t.Type.ToString(),
            t.StoryPoints, t.DueDate,
            t.ParentTaskId,
            t.OrderIndex,
            t.Comments.Count, t.Attachments.Count, t.Subtasks.Count,
            new List<LabelResponse>(),
            t.CreatedAt, t.UpdatedAt
        ));
    }

    public async Task<TaskResponse> ReorderAsync(Guid id, ReorderTaskRequest request, Guid currentUserId, CancellationToken ct = default)
    {
        var task = await _taskRepository.GetByIdAsync(id, ct);
        if (task == null) throw new NotFoundException("Task not found.");
        task.OrderIndex = request.OrderIndex;
        _taskRepository.Update(task);
        await _unitOfWork.SaveChangesAsync(ct);
        return new TaskResponse(
            task.Id, task.Title, task.Description,
            task.ProjectId, "Project",
            task.SprintId, "Sprint",
            task.AssigneeId, "Assignee",
            task.ReporterId, "Reporter",
            task.Status.ToString(), task.Priority.ToString(), task.Type.ToString(),
            task.StoryPoints, task.DueDate,
            task.ParentTaskId,
            task.OrderIndex,
            task.Comments.Count, task.Attachments.Count, task.Subtasks.Count,
            new List<LabelResponse>(),
            task.CreatedAt, task.UpdatedAt
        );
    }
}
