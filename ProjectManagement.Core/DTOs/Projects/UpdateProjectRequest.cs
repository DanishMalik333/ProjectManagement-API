namespace ProjectManagement.Core.DTOs.Projects;
public record UpdateProjectRequest(string Name, string? Description, string Status, DateOnly? StartDate, DateOnly? EndDate);
