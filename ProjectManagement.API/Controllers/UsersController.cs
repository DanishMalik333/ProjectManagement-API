using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagement.Core.DTOs.Auth;
using ProjectManagement.Core.DTOs.Users;
using ProjectManagement.Core.Interfaces;

namespace ProjectManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController(IUserService userService, ICurrentUserService currentUserService) : ControllerBase
{
    /// <summary>
    /// Get all users (paginated)
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserResponse>>> GetAll([FromQuery] string? role, [FromQuery] bool? isActive, [FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
    {
        var users = await userService.GetAllAsync(role, isActive, search, page, pageSize, ct);
        return Ok(users);
    }

    /// <summary>
    /// Get current user profile
    /// </summary>
    [HttpGet("me")]
    public async Task<ActionResult<UserResponse>> GetCurrentUser(CancellationToken ct = default)
    {
        var userId = currentUserService.UserId;
        var user = await userService.GetCurrentUserAsync(userId, ct);
        return Ok(user);
    }

    /// <summary>
    /// Get user by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<UserResponse>> GetById(Guid id, CancellationToken ct = default)
    {
        var user = await userService.GetByIdAsync(id, ct);
        return Ok(user);
    }

    /// <summary>
    /// Update user profile
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProfileRequest request, CancellationToken ct = default)
    {
        var currentUserId = currentUserService.UserId;
        if (id != currentUserId && !User.IsInRole("Admin"))
        {
            return Forbid();
        }

        await userService.UpdateProfileAsync(id, request, ct);
        return NoContent();
    }

    /// <summary>
    /// Update user role (Admin only)
    /// </summary>
    [HttpPut("{userId}/role")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateRole(Guid userId, [FromBody] UpdateUserRoleRequest request, CancellationToken ct = default)
    {
        await userService.UpdateRoleAsync(userId, request, ct);
        return NoContent();
    }

    /// <summary>
    /// Deactivate user account
    /// </summary>
    [HttpPost("{id}/deactivate")]
    public async Task<IActionResult> Deactivate(Guid id, CancellationToken ct = default)
    {
        var currentUserId = currentUserService.UserId;
        if (id != currentUserId && !User.IsInRole("Admin"))
        {
            return Forbid();
        }

        await userService.DeactivateAsync(id, ct);
        return NoContent();
    }

    /// <summary>
    /// Activate user account (Admin only)
    /// </summary>
    [HttpPost("{id}/activate")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Activate(Guid id, CancellationToken ct = default)
    {
        await userService.ActivateAsync(id, ct);
        return NoContent();
    }
}
