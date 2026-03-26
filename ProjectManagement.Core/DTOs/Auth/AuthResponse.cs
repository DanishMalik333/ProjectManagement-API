using ProjectManagement.Core.DTOs.Users;
namespace ProjectManagement.Core.DTOs.Auth;
public record AuthResponse(string AccessToken, string RefreshToken, int ExpiresIn, UserResponse User);
