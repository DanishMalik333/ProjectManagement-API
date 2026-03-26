using Microsoft.AspNetCore.Identity;
using ProjectManagement.Core.DTOs.Teams;
using ProjectManagement.Core.Entities;
using ProjectManagement.Core.Enums;
using ProjectManagement.Core.Exceptions;
using ProjectManagement.Core.Interfaces;
using ProjectManagement.Services.Extensions;

namespace ProjectManagement.Services.Services;

public class TeamService : ITeamService
{
    private readonly ITeamRepository _teamRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUnitOfWork _unitOfWork;

    public TeamService(
        ITeamRepository teamRepository,
        UserManager<ApplicationUser> userManager,
        IUnitOfWork unitOfWork)
    {
        _teamRepository = teamRepository;
        _userManager = userManager;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<TeamResponse>> GetByUserAsync(Guid userId, CancellationToken ct = default)
    {
        var teams = await _teamRepository.GetByUserIdAsync(userId, ct);
        var responses = new List<TeamResponse>();
        foreach (var t in teams)
        {
            var owner = await _userManager.FindByIdAsync(t.CreatedByUserId.ToString());
            responses.Add(new TeamResponse(t.Id, t.Name, t.Description, t.CreatedByUserId, owner?.FullName() ?? "Unknown", t.CreatedAt, t.UpdatedAt, null));
        }
        return responses;
    }

    public async Task<TeamResponse> CreateAsync(CreateTeamRequest request, Guid creatorId, CancellationToken ct = default)
    {
        var team = new Team
        {
            Name = request.Name,
            Description = request.Description ?? string.Empty,
            CreatedByUserId = creatorId
        };

        await _teamRepository.AddAsync(team, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        var creator = await _userManager.FindByIdAsync(creatorId.ToString());
        return new TeamResponse(team.Id, team.Name, team.Description, team.CreatedByUserId, creator?.FullName() ?? "Unknown", team.CreatedAt, team.UpdatedAt, null);
    }

    public async Task<TeamResponse> GetByIdAsync(Guid id, Guid currentUserId, CancellationToken ct = default)
    {
        var team = await _teamRepository.GetByIdAsync(id, ct);
        if (team == null)
            throw new NotFoundException("Team not found.");
        var creator = await _userManager.FindByIdAsync(team.CreatedByUserId.ToString());
        return new TeamResponse(team.Id, team.Name, team.Description, team.CreatedByUserId, creator?.FullName() ?? "Unknown", team.CreatedAt, team.UpdatedAt, null);
    }

    public async Task<TeamResponse> UpdateAsync(Guid id, UpdateTeamRequest request, Guid currentUserId, CancellationToken ct = default)
    {
        var team = await _teamRepository.GetByIdAsync(id, ct);
        if (team == null)
            throw new NotFoundException("Team not found.");

        if (team.CreatedByUserId != currentUserId)
            throw new ForbiddenException("Only team owner can update team.");

        team.Name = request.Name;
        team.Description = request.Description ?? team.Description;

        _teamRepository.Update(team);
        await _unitOfWork.SaveChangesAsync(ct);
        var creator = await _userManager.FindByIdAsync(team.CreatedByUserId.ToString());
        return new TeamResponse(team.Id, team.Name, team.Description, team.CreatedByUserId, creator?.FullName() ?? "Unknown", team.CreatedAt, team.UpdatedAt, null);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var team = await _teamRepository.GetByIdAsync(id, ct);
        if (team == null)
            throw new NotFoundException("Team not found.");

        _teamRepository.Delete(team);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task<IEnumerable<TeamMemberResponse>> GetMembersAsync(Guid teamId, Guid currentUserId, CancellationToken ct = default)
    {
        var team = await _teamRepository.GetWithMembersAsync(teamId, ct);
        if (team == null)
            throw new NotFoundException("Team not found.");

        var responses = new List<TeamMemberResponse>();
        foreach (var m in team.TeamMembers ?? new List<TeamMember>())
        {
            var user = await _userManager.FindByIdAsync(m.UserId.ToString());
            responses.Add(new TeamMemberResponse(m.UserId, user?.Email ?? "", user?.FullName() ?? "", m.Role.ToString(), m.JoinedAt));
        }
        return responses;
    }

    public async Task<TeamMemberResponse> AddMemberAsync(Guid teamId, AddTeamMemberRequest request, Guid currentUserId, CancellationToken ct = default)
    {
        var team = await _teamRepository.GetByIdAsync(teamId, ct);
        if (team == null)
            throw new NotFoundException("Team not found.");

        if (team.CreatedByUserId != currentUserId)
            throw new ForbiddenException("Only team owner can add members.");

        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
            throw new NotFoundException("User not found.");

        var teamMember = new TeamMember
        {
            TeamId = teamId,
            UserId = user.Id,
            Role = Enum.TryParse<TeamMemberRole>(request.Role, ignoreCase: true, out var role) ? role : TeamMemberRole.Member
        };

        await _unitOfWork.SaveChangesAsync(ct);

        return new TeamMemberResponse(user.Id, user.Email ?? "", user.FullName(), teamMember.Role.ToString(), DateTimeOffset.UtcNow);
    }

    public async Task<TeamMemberResponse> UpdateMemberRoleAsync(Guid teamId, Guid userId, UpdateTeamMemberRoleRequest request, Guid currentUserId, CancellationToken ct = default)
    {
        var team = await _teamRepository.GetByIdAsync(teamId, ct);
        if (team == null)
            throw new NotFoundException("Team not found.");

        if (team.CreatedByUserId != currentUserId)
            throw new ForbiddenException("Only team owner can update member roles.");

        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            throw new NotFoundException("User not found.");
        
        return new TeamMemberResponse(userId, user.Email ?? "", user.FullName(), request.Role, DateTimeOffset.UtcNow);
    }

    public async Task RemoveMemberAsync(Guid teamId, Guid userId, Guid currentUserId, CancellationToken ct = default)
    {
        var team = await _teamRepository.GetByIdAsync(teamId, ct);
        if (team == null)
            throw new NotFoundException("Team not found.");

        if (team.CreatedByUserId != currentUserId)
            throw new ForbiddenException("Only team owner can remove members.");

        await _unitOfWork.SaveChangesAsync(ct);
    }
}
