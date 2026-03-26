using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagement.Core.DTOs.Auth;
using ProjectManagement.Core.Interfaces;

namespace ProjectManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    /// <summary>
    /// Register a new user account
    /// </summary>
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request, CancellationToken ct = default)
    {
        var result = await authService.RegisterAsync(request, ct);
        return Created(string.Empty, result);
    }

    /// <summary>
    /// Authenticate user with email and password
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request, CancellationToken ct = default)
    {
        var result = await authService.LoginAsync(request, ct);
        return Ok(result);
    }

    /// <summary>
    /// Refresh the JWT access token using a refresh token
    /// </summary>
    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> RefreshToken([FromBody] RefreshTokenRequest request, CancellationToken ct = default)
    {
        var result = await authService.RefreshAsync(request, ct);
        return Ok(result);
    }

    /// <summary>
    /// Logout by invalidating the refresh token
    /// </summary>
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout([FromBody] RefreshTokenRequest request, CancellationToken ct = default)
    {
        await authService.LogoutAsync(request.RefreshToken, ct);
        return NoContent();
    }
}
