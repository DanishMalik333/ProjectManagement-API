namespace ProjectManagement.Core.DTOs.Tasks;
public record CreateTaskRequest(
    string Title, string? Description,
    Guid ProjectId, Guid? SprintId, Guid? AssigneeId,
    string Priority, string Type,
    int? StoryPoints, DateOnly? DueDate,
    Guid? ParentTaskId
);
