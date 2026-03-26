using ProjectManagement.Core.DTOs.Users;
namespace ProjectManagement.Core.DTOs.Teams;
public record TeamResponse(Guid Id, string Name, string? Description, Guid CreatedByUserId, string CreatedByName, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, IEnumerable<TeamMemberResponse>? Members);
