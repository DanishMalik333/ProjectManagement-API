using ProjectManagement.Core.DTOs.Projects;
namespace ProjectManagement.Core.DTOs.Tasks;
public record TaskResponse(
    Guid Id, string Title, string? Description,
    Guid ProjectId, string ProjectName,
    Guid? SprintId, string? SprintName,
    Guid? AssigneeId, string? AssigneeName,
    Guid ReporterId, string ReporterName,
    string Status, string Priority, string Type,
    int? StoryPoints, DateOnly? DueDate,
    Guid? ParentTaskId,
    int OrderIndex,
    int CommentCount, int AttachmentCount, int SubtaskCount,
    IEnumerable<LabelResponse> Labels,
    DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt
);
