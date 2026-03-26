using ProjectManagement.Core.Enums;
namespace ProjectManagement.Core.Entities;
public class TeamMember
{
    public Guid TeamId { get; set; }
    public Guid UserId { get; set; }
    public TeamMemberRole Role { get; set; }
    public DateTimeOffset JoinedAt { get; set; }
    public Team Team { get; set; } = null!;
    public ApplicationUser User { get; set; } = null!;
}
