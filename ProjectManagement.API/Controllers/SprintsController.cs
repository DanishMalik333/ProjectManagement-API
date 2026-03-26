using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagement.Core.DTOs.Sprints;
using ProjectManagement.Core.Interfaces;

namespace ProjectManagement.API.Controllers;

[ApiController]
[Route("api/projects/{projectId}/sprints")]
[Authorize]
public class SprintsController(ISprintService sprintService, ICurrentUserService currentUserService) : ControllerBase
{
    /// <summary>
    /// Get all sprints for a project
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SprintResponse>>> GetByProject(Guid projectId, [FromQuery] string? status = null, CancellationToken ct = default)
    {
        var sprints = await sprintService.GetByProjectAsync(projectId, status, currentUserService.UserId, ct);
        return Ok(sprints);
    }

    /// <summary>
    /// Create a new sprint for a project
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<SprintResponse>> Create(Guid projectId, [FromBody] CreateSprintRequest request, CancellationToken ct = default)
    {
        var sprint = await sprintService.CreateAsync(projectId, request, currentUserService.UserId, ct);
        return CreatedAtAction(nameof(GetById), new { projectId, id = sprint.Id }, sprint);
    }

    /// <summary>
    /// Get sprint by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<SprintResponse>> GetById(Guid projectId, Guid id, CancellationToken ct = default)
    {
        var sprint = await sprintService.GetByIdAsync(projectId, id, currentUserService.UserId, ct);
        return Ok(sprint);
    }

    /// <summary>
    /// Update sprint details
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid projectId, Guid id, [FromBody] UpdateSprintRequest request, CancellationToken ct = default)
    {
        await sprintService.UpdateAsync(projectId, id, request, currentUserService.UserId, ct);
        return NoContent();
    }

    /// <summary>
    /// Complete a sprint
    /// </summary>
    [HttpPost("{id}/complete")]
    public async Task<ActionResult<SprintResponse>> Complete(Guid projectId, Guid id, [FromBody] CompleteSprintRequest request, CancellationToken ct = default)
    {
        var result = await sprintService.CompleteAsync(projectId, id, request, currentUserService.UserId, ct);
        return Ok(result);
    }

    /// <summary>
    /// Delete sprint (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid projectId, Guid id, CancellationToken ct = default)
    {
        await sprintService.DeleteAsync(projectId, id, currentUserService.UserId, ct);
        return NoContent();
    }
}
