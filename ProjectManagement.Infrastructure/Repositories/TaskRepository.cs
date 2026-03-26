using Microsoft.EntityFrameworkCore;
using ProjectManagement.Core.DTOs.Common;
using ProjectManagement.Core.DTOs.Tasks;
using ProjectManagement.Core.Entities;
using ProjectManagement.Core.Enums;
using ProjectManagement.Core.Interfaces;
using ProjectManagement.Infrastructure.Persistence;
using TaskStatus = ProjectManagement.Core.Enums.TaskStatus;

namespace ProjectManagement.Infrastructure.Repositories;

public class TaskRepository : GenericRepository<ProjectTask>, ITaskRepository
{
    public TaskRepository(AppDbContext context) : base(context) { }

    public async Task<PagedResult<ProjectTask>> GetFilteredAsync(TaskFilterParams filter, CancellationToken ct = default)
    {
        var query = _context.Tasks
            .Include(t => t.Assignee)
            .Include(t => t.Reporter)
            .Include(t => t.Sprint)
            .Include(t => t.Project)
            .Include(t => t.TaskLabels).ThenInclude(tl => tl.Label)
            .Include(t => t.Comments)
            .Include(t => t.Attachments)
            .Include(t => t.Subtasks)
            .Where(t => t.ProjectId == filter.ProjectId);

        // Sprint filter: "null" string = backlog only
        if (!string.IsNullOrEmpty(filter.SprintId))
        {
            if (filter.SprintId.Equals("null", StringComparison.OrdinalIgnoreCase))
                query = query.Where(t => t.SprintId == null);
            else if (Guid.TryParse(filter.SprintId, out var sprintId))
                query = query.Where(t => t.SprintId == sprintId);
        }

        if (filter.AssigneeId.HasValue)
            query = query.Where(t => t.AssigneeId == filter.AssigneeId.Value);

        if (filter.Status?.Length > 0)
        {
            var statuses = filter.Status
                .Select(s => Enum.TryParse<TaskStatus>(s, true, out var e) ? (TaskStatus?)e : null)
                .Where(s => s.HasValue).Select(s => s!.Value).ToList();
            if (statuses.Count > 0)
                query = query.Where(t => statuses.Contains(t.Status));
        }

        if (filter.Priority?.Length > 0)
        {
            var priorities = filter.Priority
                .Select(p => Enum.TryParse<TaskPriority>(p, true, out var e) ? (TaskPriority?)e : null)
                .Where(p => p.HasValue).Select(p => p!.Value).ToList();
            if (priorities.Count > 0)
                query = query.Where(t => priorities.Contains(t.Priority));
        }

        if (filter.Type?.Length > 0)
        {
            var types = filter.Type
                .Select(ty => Enum.TryParse<TaskType>(ty, true, out var e) ? (TaskType?)e : null)
                .Where(ty => ty.HasValue).Select(ty => ty!.Value).ToList();
            if (types.Count > 0)
                query = query.Where(t => types.Contains(t.Type));
        }

        if (filter.LabelIds?.Length > 0)
            query = query.Where(t => t.TaskLabels.Any(tl => filter.LabelIds.Contains(tl.LabelId)));

        if (!string.IsNullOrEmpty(filter.Search))
            query = query.Where(t => t.Title.Contains(filter.Search) || (t.Description != null && t.Description.Contains(filter.Search)));

        if (filter.ParentTaskId.HasValue)
            query = query.Where(t => t.ParentTaskId == filter.ParentTaskId.Value);

        var total = await query.CountAsync(ct);

        query = (filter.SortBy?.ToLower(), filter.SortDirection?.ToLower()) switch
        {
            ("createdat", "desc") => query.OrderByDescending(t => t.CreatedAt),
            ("createdat", _) => query.OrderBy(t => t.CreatedAt),
            ("updatedat", "desc") => query.OrderByDescending(t => t.UpdatedAt),
            ("updatedat", _) => query.OrderBy(t => t.UpdatedAt),
            ("priority", "desc") => query.OrderByDescending(t => t.Priority),
            ("priority", _) => query.OrderBy(t => t.Priority),
            ("duedate", "desc") => query.OrderByDescending(t => t.DueDate),
            ("duedate", _) => query.OrderBy(t => t.DueDate),
            (_, "desc") => query.OrderByDescending(t => t.OrderIndex),
            _ => query.OrderBy(t => t.OrderIndex),
        };

        var items = await query.Skip((filter.Page - 1) * filter.PageSize).Take(filter.PageSize).ToListAsync(ct);
        var totalPages = (int)Math.Ceiling(total / (double)filter.PageSize);

        return new PagedResult<ProjectTask>(items, total, filter.Page, filter.PageSize, totalPages);
    }

    public async Task<ProjectTask?> GetWithDetailsAsync(Guid id, CancellationToken ct = default) =>
        await _context.Tasks
            .Include(t => t.Assignee)
            .Include(t => t.Reporter)
            .Include(t => t.Sprint)
            .Include(t => t.Project)
            .Include(t => t.TaskLabels).ThenInclude(tl => tl.Label)
            .Include(t => t.Comments)
            .Include(t => t.Attachments)
            .Include(t => t.Subtasks)
            .FirstOrDefaultAsync(t => t.Id == id, ct);

    public async Task<IEnumerable<ProjectTask>> GetSubtasksAsync(Guid parentId, CancellationToken ct = default) =>
        await _context.Tasks
            .Include(t => t.Assignee)
            .Include(t => t.Reporter)
            .Where(t => t.ParentTaskId == parentId)
            .ToListAsync(ct);

    public async Task<int> CountByStatusAsync(Guid projectId, TaskStatus status, CancellationToken ct = default) =>
        await _context.Tasks
            .CountAsync(t => t.ProjectId == projectId && t.Status == status, ct);

    public async Task<IEnumerable<ProjectTask>> GetBySprintAsync(Guid sprintId, CancellationToken ct = default) =>
        await _context.Tasks
            .Where(t => t.SprintId == sprintId)
            .ToListAsync(ct);
}
