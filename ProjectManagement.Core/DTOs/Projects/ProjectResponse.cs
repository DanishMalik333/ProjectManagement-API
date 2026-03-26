namespace ProjectManagement.Core.DTOs.Projects;
public record ProjectResponse(Guid Id, string Name, string? Description, string Key, string Status, Guid TeamId, string TeamName, Guid OwnerId, string OwnerName, DateOnly? StartDate, DateOnly? EndDate, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt);
