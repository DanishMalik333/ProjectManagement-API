namespace ProjectManagement.Core.DTOs.Tasks;
public record UpdateTaskRequest(
    string Title, string? Description,
    string Priority, string Type,
    int? StoryPoints, DateOnly? DueDate
);
