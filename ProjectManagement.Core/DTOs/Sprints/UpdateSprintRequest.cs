namespace ProjectManagement.Core.DTOs.Sprints;
public record UpdateSprintRequest(string Name, string? Goal, DateOnly StartDate, DateOnly EndDate);
