namespace ProjectManagement.Core.DTOs.Auth;
public record UpdateProfileRequest(string FirstName, string LastName, string? PhoneNumber, string? ProfilePictureUrl);
