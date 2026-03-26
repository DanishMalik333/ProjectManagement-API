using ProjectManagement.Core.Enums;
using ProjectManagement.Core.Interfaces;
namespace ProjectManagement.Core.Entities;
public class ProjectTask : IAuditableEntity, ISoftDeletable
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid ProjectId { get; set; }
    public Guid? SprintId { get; set; }
    public Guid? AssigneeId { get; set; }
    public Guid ReporterId { get; set; }
    public Enums.TaskStatus Status { get; set; } = Enums.TaskStatus.Backlog;
    public Enums.TaskPriority Priority { get; set; } = Enums.TaskPriority.Medium;
    public Enums.TaskType Type { get; set; } = Enums.TaskType.Task;
    public int? StoryPoints { get; set; }
    public DateOnly? DueDate { get; set; }
    public Guid? ParentTaskId { get; set; }
    public int OrderIndex { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public Project Project { get; set; } = null!;
    public Sprint? Sprint { get; set; }
    public ApplicationUser? Assignee { get; set; }
    public ApplicationUser Reporter { get; set; } = null!;
    public ProjectTask? ParentTask { get; set; }
    public ICollection<ProjectTask> Subtasks { get; set; } = new List<ProjectTask>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<TaskLabel> TaskLabels { get; set; } = new List<TaskLabel>();
    public ICollection<TaskAttachment> Attachments { get; set; } = new List<TaskAttachment>();
    public ICollection<TaskHistory> History { get; set; } = new List<TaskHistory>();
}
