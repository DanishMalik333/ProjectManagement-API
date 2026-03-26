using ProjectManagement.Core.Interfaces;

namespace ProjectManagement.Core.Entities;

public class Label : IAuditableEntity, ISoftDeletable
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public Guid ProjectId { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public Project Project { get; set; } = null!;
    public ICollection<TaskLabel> TaskLabels { get; set; } = new List<TaskLabel>();
}
