using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagement.Core.DTOs.Tasks;
using ProjectManagement.Core.Interfaces;

namespace ProjectManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TasksController(ITaskService taskService, ICurrentUserService currentUserService) : ControllerBase
{
    /// <summary>
    /// Get tasks with optional filtering
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskResponse>>> GetFiltered([FromQuery] TaskFilterParams filterParams, CancellationToken ct = default)
    {
        var tasks = await taskService.GetFilteredAsync(filterParams, ct);
        return Ok(tasks);
    }

    /// <summary>
    /// Create a new task
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<TaskResponse>> Create([FromBody] CreateTaskRequest request, CancellationToken ct = default)
    {
        var userId = currentUserService.UserId;
        var task = await taskService.CreateAsync(request, userId, ct);
        return CreatedAtAction(nameof(GetById), new { id = task.Id }, task);
    }

    /// <summary>
    /// Get task by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<TaskResponse>> GetById(Guid id, CancellationToken ct = default)
    {
        var task = await taskService.GetByIdAsync(id, currentUserService.UserId, ct);
        return Ok(task);
    }

    /// <summary>
    /// Update task details
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTaskRequest request, CancellationToken ct = default)
    {
        await taskService.UpdateAsync(id, request, currentUserService.UserId, ct);
        return NoContent();
    }

    /// <summary>
    /// Update task status
    /// </summary>
    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateTaskStatusRequest request, CancellationToken ct = default)
    {
        var userId = currentUserService.UserId;
        await taskService.UpdateStatusAsync(id, request, userId, ct);
        return NoContent();
    }

    /// <summary>
    /// Assign task to a user
    /// </summary>
    [HttpPut("{id}/assign")]
    public async Task<IActionResult> UpdateAssignee(Guid id, [FromBody] UpdateAssigneeRequest request, CancellationToken ct = default)
    {
        await taskService.UpdateAssigneeAsync(id, request, currentUserService.UserId, ct);
        return NoContent();
    }

    /// <summary>
    /// Add label to task
    /// </summary>
    [HttpPost("{taskId}/labels")]
    public async Task<IActionResult> AddLabel(Guid taskId, [FromBody] AddLabelToTaskRequest request, CancellationToken ct = default)
    {
        await taskService.AddLabelAsync(taskId, request, currentUserService.UserId, ct);
        return NoContent();
    }

    /// <summary>
    /// Remove label from task
    /// </summary>
    [HttpDelete("{taskId}/labels/{labelId}")]
    public async Task<IActionResult> RemoveLabel(Guid taskId, Guid labelId, CancellationToken ct = default)
    {
        await taskService.RemoveLabelAsync(taskId, labelId, currentUserService.UserId, ct);
        return NoContent();
    }

    /// <summary>
    /// Reorder tasks within a sprint
    /// </summary>
    [HttpPost("{taskId}/reorder")]
    public async Task<IActionResult> Reorder(Guid taskId, [FromBody] ReorderTaskRequest request, CancellationToken ct = default)
    {
        await taskService.ReorderAsync(taskId, request, currentUserService.UserId, ct);
        return NoContent();
    }

    /// <summary>
    /// Delete task (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct = default)
    {
        await taskService.DeleteAsync(id, currentUserService.UserId, ct);
        return NoContent();
    }
}
