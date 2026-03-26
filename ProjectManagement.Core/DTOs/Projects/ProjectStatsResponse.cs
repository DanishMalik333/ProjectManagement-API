namespace ProjectManagement.Core.DTOs.Projects;
public record ProjectStatsResponse(Dictionary<string, int> TaskCountByStatus, Dictionary<string, int> TaskCountByPriority, double CompletionPercent, Guid? ActiveSprintId);
