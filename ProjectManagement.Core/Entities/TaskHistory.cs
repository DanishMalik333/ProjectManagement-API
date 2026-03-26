using ProjectManagement.Core.Interfaces;

namespace ProjectManagement.Core.Entities;

public class TaskHistory : IAuditableEntity, ISoftDeletable
{
    public Guid Id { get; set; }
    public Guid TaskId { get; set; }
    public Guid ChangedByUserId { get; set; }
    public string FieldName { get; set; } = string.Empty;
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public DateTimeOffset ChangedAt { get; set; }
    public ProjectTask Task { get; set; } = null!;
    public ApplicationUser ChangedBy { get; set; } = null!;
}
