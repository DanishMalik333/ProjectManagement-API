namespace ProjectManagement.Core.DTOs.Auth;
public record ChangePasswordRequest(string CurrentPassword, string NewPassword);
