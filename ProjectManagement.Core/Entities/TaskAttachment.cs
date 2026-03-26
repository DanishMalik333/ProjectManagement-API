using ProjectManagement.Core.Interfaces;

namespace ProjectManagement.Core.Entities;

public class TaskAttachment : IAuditableEntity, ISoftDeletable
{
    public Guid Id { get; set; }
    public Guid TaskId { get; set; }
    public Guid UploadedByUserId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public bool IsDeleted { get; set; } = false;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public DateTimeOffset UploadedAt { get; set; }
    public ProjectTask Task { get; set; } = null!;
    public ApplicationUser UploadedBy { get; set; } = null!;
}
