using ProjectManagement.Core.Enums;
using ProjectManagement.Core.Interfaces;

namespace ProjectManagement.Core.Entities;

public class Sprint : IAuditableEntity, ISoftDeletable
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Goal { get; set; }
    public Guid ProjectId { get; set; }
    public SprintStatus Status { get; set; } = SprintStatus.Planning;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public Project Project { get; set; } = null!;
    public ICollection<ProjectTask> Tasks { get; set; } = new List<ProjectTask>();
}
