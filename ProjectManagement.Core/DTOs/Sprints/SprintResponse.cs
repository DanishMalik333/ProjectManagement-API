namespace ProjectManagement.Core.DTOs.Sprints;
public record SprintResponse(Guid Id, string Name, string? Goal, Guid ProjectId, string Status, DateOnly StartDate, DateOnly EndDate, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, int? TaskCount);
