namespace ProjectManagement.Core.DTOs.Tasks;
public record TaskHistoryResponse(Guid Id, Guid TaskId, Guid ChangedByUserId, string ChangedByName, string FieldName, string? OldValue, string? NewValue, DateTimeOffset ChangedAt);
