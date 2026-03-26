namespace ProjectManagement.Core.DTOs.Sprints;
public record CreateSprintRequest(string Name, string? Goal, DateOnly StartDate, DateOnly EndDate);
