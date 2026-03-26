using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagement.Core.DTOs.Teams;
using ProjectManagement.Core.Interfaces;

namespace ProjectManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TeamsController(ITeamService teamService, ICurrentUserService currentUserService) : ControllerBase
{
    /// <summary>
    /// Get all teams (paginated)
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TeamResponse>>> GetAll(CancellationToken ct = default)
    {
        var userId = currentUserService.UserId;
        var teams = await teamService.GetByUserAsync(userId, ct);
        return Ok(teams);
    }

    /// <summary>
    /// Create a new team
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<TeamResponse>> Create([FromBody] CreateTeamRequest request, CancellationToken ct = default)
    {
        var userId = currentUserService.UserId;
        var team = await teamService.CreateAsync(request, userId, ct);
        return CreatedAtAction(nameof(GetById), new { id = team.Id }, team);
    }

    /// <summary>
    /// Get team by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<TeamResponse>> GetById(Guid id, CancellationToken ct = default)
    {
        var team = await teamService.GetByIdAsync(id, currentUserService.UserId, ct);
        return Ok(team);
    }

    /// <summary>
    /// Update team details
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTeamRequest request, CancellationToken ct = default)
    {
        await teamService.UpdateAsync(id, request, currentUserService.UserId, ct);
        return NoContent();
    }

    /// <summary>
    /// Delete team (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct = default)
    {
        await teamService.DeleteAsync(id, ct);
        return NoContent();
    }

    /// <summary>
    /// Add member to team
    /// </summary>
    [HttpPost("{teamId}/members")]
    public async Task<ActionResult<TeamMemberResponse>> AddMember(Guid teamId, [FromBody] AddTeamMemberRequest request, CancellationToken ct = default)
    {
        var member = await teamService.AddMemberAsync(teamId, request, currentUserService.UserId, ct);
        return Created(string.Empty, member);
    }

    /// <summary>
    /// Get all team members
    /// </summary>
    [HttpGet("{teamId}/members")]
    public async Task<ActionResult<IEnumerable<TeamMemberResponse>>> GetMembers(Guid teamId, CancellationToken ct = default)
    {
        var members = await teamService.GetMembersAsync(teamId, currentUserService.UserId, ct);
        return Ok(members);
    }

    /// <summary>
    /// Update team member role
    /// </summary>
    [HttpPut("{teamId}/members/{memberId}/role")]
    public async Task<IActionResult> UpdateMemberRole(Guid teamId, Guid memberId, [FromBody] UpdateTeamMemberRoleRequest request, CancellationToken ct = default)
    {
        await teamService.UpdateMemberRoleAsync(teamId, memberId, request, currentUserService.UserId, ct);
        return NoContent();
    }

    /// <summary>
    /// Remove member from team
    /// </summary>
    [HttpDelete("{teamId}/members/{memberId}")]
    public async Task<IActionResult> RemoveMember(Guid teamId, Guid memberId, CancellationToken ct = default)
    {
        await teamService.RemoveMemberAsync(teamId, memberId, currentUserService.UserId, ct);
        return NoContent();
    }
}
