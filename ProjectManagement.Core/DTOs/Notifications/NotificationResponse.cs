namespace ProjectManagement.Core.DTOs.Notifications;
public record NotificationResponse(Guid Id, Guid UserId, string Type, string Message, Guid ReferenceId, string ReferenceType, bool IsRead, DateTimeOffset CreatedAt);
