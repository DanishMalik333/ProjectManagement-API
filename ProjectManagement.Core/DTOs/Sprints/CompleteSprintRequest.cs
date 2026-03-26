namespace ProjectManagement.Core.DTOs.Sprints;
public record CompleteSprintRequest(string IncompleteTaskAction, Guid? NextSprintId);
