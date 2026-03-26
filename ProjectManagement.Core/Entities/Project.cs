using ProjectManagement.Core.Enums;
using ProjectManagement.Core.Interfaces;
namespace ProjectManagement.Core.Entities;
public class Project : IAuditableEntity, ISoftDeletable
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Key { get; set; } = string.Empty;
    public ProjectStatus Status { get; set; } = ProjectStatus.Active;
    public Guid TeamId { get; set; }
    public Guid OwnerId { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public Team Team { get; set; } = null!;
    public ApplicationUser Owner { get; set; } = null!;
    public ICollection<Sprint> Sprints { get; set; } = new List<Sprint>();
    public ICollection<ProjectTask> Tasks { get; set; } = new List<ProjectTask>();
    public ICollection<Label> Labels { get; set; } = new List<Label>();
}
