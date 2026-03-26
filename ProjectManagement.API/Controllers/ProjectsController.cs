using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagement.Core.DTOs.Projects;
using ProjectManagement.Core.Interfaces;

namespace ProjectManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProjectsController(IProjectService projectService, ICurrentUserService currentUserService) : ControllerBase
{
    /// <summary>
    /// Get all projects (paginated)
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProjectResponse>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? teamId = null, [FromQuery] string? status = null, CancellationToken ct = default)
    {
        var projects = await projectService.GetAccessibleAsync(currentUserService.UserId, teamId, status, page, pageSize, ct);
        return Ok(projects);
    }

    /// <summary>
    /// Create a new project
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ProjectResponse>> Create([FromBody] CreateProjectRequest request, CancellationToken ct = default)
    {
        var userId = currentUserService.UserId;
        var project = await projectService.CreateAsync(request, userId, ct);
        return CreatedAtAction(nameof(GetById), new { id = project.Id }, project);
    }

    /// <summary>
    /// Get project by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ProjectResponse>> GetById(Guid id, CancellationToken ct = default)
    {
        var project = await projectService.GetByIdAsync(id, currentUserService.UserId, ct);
        return Ok(project);
    }

    /// <summary>
    /// Update project details
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProjectRequest request, CancellationToken ct = default)
    {
        await projectService.UpdateAsync(id, request, currentUserService.UserId, ct);
        return NoContent();
    }

    /// <summary>
    /// Delete project (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct = default)
    {
        await projectService.DeleteAsync(id, ct);
        return NoContent();
    }

    /// <summary>
    /// Get project statistics
    /// </summary>
    [HttpGet("{id}/statistics")]
    public async Task<ActionResult<ProjectStatsResponse>> GetStatistics(Guid id, CancellationToken ct = default)
    {
        var stats = await projectService.GetStatsAsync(id, currentUserService.UserId, ct);
        return Ok(stats);
    }

    /// <summary>
    /// Create a label for the project
    /// </summary>
    [HttpPost("{projectId}/labels")]
    public async Task<ActionResult<LabelResponse>> CreateLabel(Guid projectId, [FromBody] CreateLabelRequest request, CancellationToken ct = default)
    {
        var label = await projectService.CreateLabelAsync(projectId, request, ct);
        return Created(string.Empty, label);
    }

    /// <summary>
    /// Get all project labels
    /// </summary>
    [HttpGet("{projectId}/labels")]
    public async Task<ActionResult<IEnumerable<LabelResponse>>> GetLabels(Guid projectId, CancellationToken ct = default)
    {
        var labels = await projectService.GetLabelsAsync(projectId, currentUserService.UserId, ct);
        return Ok(labels);
    }

    /// <summary>
    /// Delete a label
    /// </summary>
    [HttpDelete("labels/{labelId}")]
    public async Task<IActionResult> DeleteLabel(Guid labelId, CancellationToken ct = default)
    {
        // Note: DeleteLabelAsync requires projectId context
        // This endpoint design needs route parameter adjustment
        return StatusCode(501, "Not implemented");
    }
}
