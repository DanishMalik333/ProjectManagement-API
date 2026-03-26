namespace ProjectManagement.Core.DTOs.Projects;
public record CreateProjectRequest(string Name, string? Description, string Key, Guid TeamId, DateOnly? StartDate, DateOnly? EndDate);
