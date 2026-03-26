using ProjectManagement.Core.Enums;
using ProjectManagement.Core.Interfaces;

namespace ProjectManagement.Core.Entities;

public class Notification : IAuditableEntity, ISoftDeletable
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public NotificationType Type { get; set; }
    public string Message { get; set; } = string.Empty;
    public Guid ReferenceId { get; set; }
    public string ReferenceType { get; set; } = string.Empty;
    public bool IsRead { get; set; } = false;
    public bool IsDeleted { get; set; } = false;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    
    public ApplicationUser User { get; set; } = null!;
}
