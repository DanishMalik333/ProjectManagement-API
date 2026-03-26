using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagement.Core.DTOs.Attachments;
using ProjectManagement.Core.Interfaces;

namespace ProjectManagement.API.Controllers;

[ApiController]
[Route("api/tasks/{taskId}/attachments")]
[Authorize]
public class AttachmentsController(IAttachmentService attachmentService, ICurrentUserService currentUserService) : ControllerBase
{
    /// <summary>
    /// Get all attachments for a task
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AttachmentResponse>>> GetByTask(Guid taskId, CancellationToken ct = default)
    {
        var attachments = await attachmentService.GetByTaskAsync(taskId, ct);
        return Ok(attachments);
    }

    /// <summary>
    /// Upload a file attachment to a task
    /// </summary>
    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<AttachmentResponse>> Upload(Guid taskId, [FromForm] IFormFile file, CancellationToken ct = default)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("File is required");
        }

        var userId = currentUserService.UserId;
        var attachment = await attachmentService.UploadAsync(taskId, file, userId, ct);
        return CreatedAtAction(nameof(GetById), new { taskId, id = attachment.Id }, attachment);
    }

    /// <summary>
    /// Get attachment by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<AttachmentResponse>> GetById(Guid taskId, Guid id, CancellationToken ct = default)
    {
        var attachments = await attachmentService.GetByTaskAsync(taskId, ct);
        var attachment = attachments.FirstOrDefault(a => a.Id == id);
        return Ok(attachment);
    }

    /// <summary>
    /// Delete attachment
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid taskId, Guid id, CancellationToken ct = default)
    {
        await attachmentService.DeleteAsync(taskId, id, currentUserService.UserId, ct);
        return NoContent();
    }
}
