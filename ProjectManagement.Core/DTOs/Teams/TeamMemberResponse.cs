namespace ProjectManagement.Core.DTOs.Teams;
public record TeamMemberResponse(Guid UserId, string Email, string FullName, string Role, DateTimeOffset JoinedAt);
