namespace ProjectManagement.Core.Constants;
public static class ValidationMessages
{
    public const string Required = "This field is required.";
    public const string TooLong = "Value exceeds maximum length.";
    public const string InvalidEmail = "Invalid email address.";
    public const string InvalidPassword = "Password must be at least 8 characters with uppercase, digit, and special character.";
    public const string InvalidStoryPoints = "Story points must be one of: 1, 2, 3, 5, 8, 13, 21.";
    public const string InvalidProjectKey = "Project key must be 2-10 uppercase letters only.";
    public const string InvalidColorFormat = "Color must be a valid hex color code (e.g., #FF5733).";
    public const string InvalidStatusTransition = "This status transition is not allowed.";
}
