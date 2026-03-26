using ProjectManagement.Core.DTOs.Teams;
namespace ProjectManagement.Core.Interfaces;
public interface ITeamService
{
    Task<IEnumerable<TeamResponse>> GetByUserAsync(Guid userId, CancellationToken ct = default);
    Task<TeamResponse> CreateAsync(CreateTeamRequest request, Guid creatorId, CancellationToken ct = default);
    Task<TeamResponse> GetByIdAsync(Guid id, Guid currentUserId, CancellationToken ct = default);
    Task<TeamResponse> UpdateAsync(Guid id, UpdateTeamRequest request, Guid currentUserId, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<TeamMemberResponse>> GetMembersAsync(Guid teamId, Guid currentUserId, CancellationToken ct = default);
    Task<TeamMemberResponse> AddMemberAsync(Guid teamId, AddTeamMemberRequest request, Guid currentUserId, CancellationToken ct = default);
    Task<TeamMemberResponse> UpdateMemberRoleAsync(Guid teamId, Guid userId, UpdateTeamMemberRoleRequest request, Guid currentUserId, CancellationToken ct = default);
    Task RemoveMemberAsync(Guid teamId, Guid userId, Guid currentUserId, CancellationToken ct = default);
}
