namespace ProjectManagement.Core.DTOs.Attachments;
public record AttachmentResponse(Guid Id, Guid TaskId, Guid UploadedByUserId, string UploadedByName, string FileName, string FileUrl, long FileSizeBytes, string ContentType, DateTimeOffset UploadedAt);
