namespace ProjectManagement.Core.DTOs.Tasks;
public record TaskFilterParams(
    Guid ProjectId,
    string? SprintId,
    Guid? AssigneeId,
    string[]? Status,
    string[]? Priority,
    string[]? Type,
    Guid[]? LabelIds,
    string? Search,
    Guid? ParentTaskId,
    int Page = 1,
    int PageSize = 20,
    string SortBy = "orderIndex",
    string SortDirection = "asc"
);
