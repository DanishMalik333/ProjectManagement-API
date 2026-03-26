using ProjectManagement.Core.Interfaces;

namespace ProjectManagement.Core.Entities;

public class TaskLabel : ISoftDeletable
{
    public Guid TaskId { get; set; }
    public Guid LabelId { get; set; }
    public bool IsDeleted { get; set; } = false;
    public ProjectTask Task { get; set; } = null!;
    public Label Label { get; set; } = null!;
}
