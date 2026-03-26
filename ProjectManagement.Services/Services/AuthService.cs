using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ProjectManagement.Core.DTOs.Auth;
using ProjectManagement.Core.DTOs.Users;
using ProjectManagement.Core.Entities;
using ProjectManagement.Core.Exceptions;
using ProjectManagement.Core.Interfaces;

namespace ProjectManagement.Services.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        IRefreshTokenRepository refreshTokenRepository,
        IUnitOfWork unitOfWork,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _refreshTokenRepository = refreshTokenRepository;
        _unitOfWork = unitOfWork;
        _configuration = configuration;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken ct)
    {
        var userExists = await _userManager.FindByEmailAsync(request.Email);
        if (userExists != null)
            throw new ConflictException("User with this email already exists.");

        var user = new ApplicationUser
        {
            Email = request.Email,
            UserName = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            IsActive = true
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
            throw new BusinessRuleViolationException($"User registration failed: {string.Join(", ", result.Errors.Select(e => e.Description))}");

        // Assign Developer role by default
        await _userManager.AddToRoleAsync(user, "Developer");

        var accessToken = GenerateAccessToken(user, new[] { "Developer" });
        var refreshToken = GenerateRefreshToken();
        var refreshTokenHash = BCrypt.Net.BCrypt.HashPassword(refreshToken);

        var refreshTokenEntity = new RefreshToken
        {
            UserId = user.Id,
            TokenHash = refreshTokenHash,
            ExpiresAt = DateTimeOffset.UtcNow.AddDays(7),
            IsRevoked = false
        };

        await _refreshTokenRepository.AddAsync(refreshTokenEntity, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        var userResponse = new UserResponse(user.Id, user.Email, user.FirstName, user.LastName, $"{user.FirstName} {user.LastName}", user.ProfilePictureUrl, user.IsActive, "Developer", user.CreatedAt);

        return new AuthResponse(accessToken, refreshToken, 3600, userResponse);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
            throw new UnauthorizedException("Invalid email or password.");

        if (!user.IsActive)
            throw new ForbiddenException("User account is deactivated.");

        if (await _userManager.IsLockedOutAsync(user))
            throw new ForbiddenException("Account is locked due to too many failed login attempts.");

        var passwordValid = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!passwordValid)
        {
            await _userManager.AccessFailedAsync(user);
            throw new UnauthorizedException("Invalid email or password.");
        }

        await _userManager.ResetAccessFailedCountAsync(user);

        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = GenerateAccessToken(user, roles.ToArray());
        var refreshToken = GenerateRefreshToken();
        var refreshTokenHash = BCrypt.Net.BCrypt.HashPassword(refreshToken);

        var refreshTokenEntity = new RefreshToken
        {
            UserId = user.Id,
            TokenHash = refreshTokenHash,
            ExpiresAt = DateTimeOffset.UtcNow.AddDays(7),
            IsRevoked = false
        };

        await _refreshTokenRepository.AddAsync(refreshTokenEntity, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        var userResponse = new UserResponse(user.Id, user.Email, user.FirstName, user.LastName, $"{user.FirstName} {user.LastName}", user.ProfilePictureUrl, user.IsActive, roles.FirstOrDefault() ?? "Developer", user.CreatedAt);

        return new AuthResponse(accessToken, refreshToken, 3600, userResponse);
    }

    public async Task<AuthResponse> RefreshAsync(RefreshTokenRequest request, CancellationToken ct)
    {
        // Hash incoming token and look up
        var tokenHash = BCrypt.Net.BCrypt.HashPassword(request.RefreshToken);
        
        // Find all tokens for this token (checking hash is complex, so search by finding potential matches)
        var allTokens = await _refreshTokenRepository.GetAllAsync(ct);
        var refreshTokens = allTokens.Where(t => !t.IsRevoked && t.ExpiresAt > DateTimeOffset.UtcNow).ToList();

        RefreshToken? storedToken = null;
        foreach (var token in refreshTokens)
        {
            if (BCrypt.Net.BCrypt.Verify(request.RefreshToken, token.TokenHash))
            {
                storedToken = token;
                break;
            }
        }

        if (storedToken == null)
        {
            // Token not found or revoked/expired
            throw new UnauthorizedException("Invalid or expired refresh token.");
        }

        if (storedToken.IsRevoked)
        {
            // Revoked token presented - detect reuse and revoke entire chain
            var chainToRevoke = refreshTokens.Where(t => 
                t.UserId == storedToken.UserId && 
                t.CreatedAt >= storedToken.CreatedAt
            ).ToList();
            
            foreach (var token in chainToRevoke)
            {
                token.IsRevoked = true;
            }
            
            await _unitOfWork.SaveChangesAsync(ct);
            throw new UnauthorizedException("Refresh token has been revoked (reuse detected).");
        }

        // Token is valid - implement rotation
        var user = await _userManager.FindByIdAsync(storedToken.UserId.ToString());
        if (user == null || !user.IsActive)
            throw new UnauthorizedException("User not found or inactive.");

        var roles = await _userManager.GetRolesAsync(user);
        var newAccessToken = GenerateAccessToken(user, roles.ToArray());
        var newRefreshToken = GenerateRefreshToken();
        var newRefreshTokenHash = BCrypt.Net.BCrypt.HashPassword(newRefreshToken);

        // Create new token and set ReplacedByTokenId on old token
        var newTokenEntity = new RefreshToken
        {
            UserId = storedToken.UserId,
            TokenHash = newRefreshTokenHash,
            ExpiresAt = DateTimeOffset.UtcNow.AddDays(7),
            IsRevoked = false
        };

        await _refreshTokenRepository.AddAsync(newTokenEntity, ct);
        
        storedToken.IsRevoked = true;
        storedToken.ReplacedByTokenId = newTokenEntity.Id;
        _refreshTokenRepository.Update(storedToken);

        await _unitOfWork.SaveChangesAsync(ct);

        var userResponse = new UserResponse(user.Id, user.Email, user.FirstName, user.LastName, $"{user.FirstName} {user.LastName}", user.ProfilePictureUrl, user.IsActive, roles.FirstOrDefault() ?? "Developer", user.CreatedAt);
        return new AuthResponse(newAccessToken, newRefreshToken, 3600, userResponse);
    }

    public async Task LogoutAsync(string refreshToken, CancellationToken ct)
    {
        // Find and revoke the refresh token
        var allTokens = await _refreshTokenRepository.GetAllAsync(ct);
        
        foreach (var token in allTokens.Where(t => !t.IsRevoked))
        {
            if (BCrypt.Net.BCrypt.Verify(refreshToken, token.TokenHash))
            {
                token.IsRevoked = true;
                _refreshTokenRepository.Update(token);
                break;
            }
        }

        await _unitOfWork.SaveChangesAsync(ct);
    }

    private string GenerateAccessToken(ApplicationUser user, string[] roles)
    {
        var jwtSettings = _configuration.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email!),
            new(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
            new("IsActive", user.IsActive.ToString())
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}
