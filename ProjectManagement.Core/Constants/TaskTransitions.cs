using ProjectManagement.Core.Enums;
namespace ProjectManagement.Core.Constants;
public static class TaskTransitions
{
    public static readonly Dictionary<Enums.TaskStatus, Enums.TaskStatus[]> Allowed = new()
    {
        [Enums.TaskStatus.Backlog]    = [Enums.TaskStatus.Todo, Enums.TaskStatus.Cancelled],
        [Enums.TaskStatus.Todo]       = [Enums.TaskStatus.InProgress, Enums.TaskStatus.Backlog, Enums.TaskStatus.Cancelled],
        [Enums.TaskStatus.InProgress] = [Enums.TaskStatus.InReview, Enums.TaskStatus.Todo, Enums.TaskStatus.Cancelled],
        [Enums.TaskStatus.InReview]   = [Enums.TaskStatus.Done, Enums.TaskStatus.InProgress, Enums.TaskStatus.Cancelled],
        [Enums.TaskStatus.Done]       = [Enums.TaskStatus.InProgress],
        [Enums.TaskStatus.Cancelled]  = [Enums.TaskStatus.Backlog],
    };

    public static bool IsAllowed(Enums.TaskStatus from, Enums.TaskStatus to) =>
        Allowed.TryGetValue(from, out var allowed) && allowed.Contains(to);
}
