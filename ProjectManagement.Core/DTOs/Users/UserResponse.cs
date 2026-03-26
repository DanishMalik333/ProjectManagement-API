namespace ProjectManagement.Core.DTOs.Users;
public record UserResponse(Guid Id, string Email, string FirstName, string LastName, string FullName, string? ProfilePictureUrl, bool IsActive, string Role, DateTimeOffset CreatedAt);
