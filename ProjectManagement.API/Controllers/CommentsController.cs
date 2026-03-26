using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagement.Core.DTOs.Comments;
using ProjectManagement.Core.Interfaces;

namespace ProjectManagement.API.Controllers;

[ApiController]
[Route("api/tasks/{taskId}/comments")]
[Authorize]
public class CommentsController(ICommentService commentService, ICurrentUserService currentUserService) : ControllerBase
{
    /// <summary>
    /// Get all comments for a task (threaded)
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CommentResponse>>> GetThreaded(Guid taskId, CancellationToken ct = default)
    {
        var comments = await commentService.GetThreadedAsync(taskId, currentUserService.UserId, ct);
        return Ok(comments);
    }

    /// <summary>
    /// Create a new comment on a task
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<CommentResponse>> Create(Guid taskId, [FromBody] CreateCommentRequest request, CancellationToken ct = default)
    {
        var userId = currentUserService.UserId;
        var comment = await commentService.CreateAsync(taskId, request, userId, ct);
        return CreatedAtAction(nameof(GetById), new { taskId, id = comment.Id }, comment);
    }

    /// <summary>
    /// Get all comments for a specific task
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<CommentResponse>> GetById(Guid taskId, Guid id, CancellationToken ct = default)
    {
        var comments = await commentService.GetThreadedAsync(taskId, currentUserService.UserId, ct);
        var comment = comments.FirstOrDefault(c => c.Id == id);
        return Ok(comment);
    }

    /// <summary>
    /// Update a comment
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid taskId, Guid id, [FromBody] UpdateCommentRequest request, CancellationToken ct = default)
    {
        var userId = currentUserService.UserId;
        await commentService.UpdateAsync(taskId, id, request, userId, ct);
        return NoContent();
    }

    /// <summary>
    /// Delete a comment
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid taskId, Guid id, CancellationToken ct = default)
    {
        await commentService.DeleteAsync(taskId, id, currentUserService.UserId, ct);
        return NoContent();
    }
}
