using Microsoft.AspNetCore.Identity;
using ProjectManagement.Core.DTOs.Auth;
using ProjectManagement.Core.DTOs.Common;
using ProjectManagement.Core.DTOs.Users;
using ProjectManagement.Core.Entities;
using ProjectManagement.Core.Exceptions;
using ProjectManagement.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Services.Extensions;

namespace ProjectManagement.Services.Services;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UserService(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        IRefreshTokenRepository refreshTokenRepository,
        IUnitOfWork unitOfWork)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _refreshTokenRepository = refreshTokenRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResult<UserResponse>> GetAllAsync(string? role, bool? isActive, string? search, int page, int pageSize, CancellationToken ct = default)
    {
        var query = _userManager.Users.AsQueryable();

        // Filter by role if provided
        if (!string.IsNullOrWhiteSpace(role))
        {
            var usersInRole = await _userManager.GetUsersInRoleAsync(role);
            var userIds = usersInRole.Select(u => u.Id).ToHashSet();
            query = query.Where(u => userIds.Contains(u.Id));
        }

        // Filter by active status
        if (isActive.HasValue)
        {
            query = query.Where(u => u.IsActive == isActive.Value);
        }

        // Filter by search term
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(u =>
                u.FirstName.Contains(search) ||
                u.LastName.Contains(search) ||
                u.Email!.Contains(search)
            );
        }

        var totalCount = query.Count();
        var users = await query
            .OrderBy(u => u.FirstName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var userResponses = new List<UserResponse>();
        foreach (var u in users)
        {
            var userRoles = await _userManager.GetRolesAsync(u);
            userResponses.Add(new UserResponse(u.Id, u.Email, u.FirstName, u.LastName, $"{u.FirstName} {u.LastName}", u.ProfilePictureUrl, u.IsActive, userRoles.FirstOrDefault() ?? "Developer", u.CreatedAt));
        }

        var totalPages = (totalCount + pageSize - 1) / pageSize;
        return new PagedResult<UserResponse>(userResponses, totalCount, page, pageSize, totalPages);
    }

    public async Task<UserResponse> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null)
            throw new NotFoundException("User not found.");
        var userRoles = await _userManager.GetRolesAsync(user);
        return new UserResponse(user.Id, user.Email, user.FirstName, user.LastName, $"{user.FirstName} {user.LastName}", user.ProfilePictureUrl, user.IsActive, userRoles.FirstOrDefault() ?? "Developer", user.CreatedAt);
    }

    public async Task UpdateProfileAsync(Guid userId, UpdateProfileRequest request, CancellationToken ct = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            throw new NotFoundException("User not found.");

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.PhoneNumber = request.PhoneNumber;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            throw new BusinessRuleViolationException("Failed to update user profile.");

        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task ChangePasswordAsync(Guid userId, ChangePasswordRequest request, CancellationToken ct = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            throw new NotFoundException("User not found.");

        var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
        if (!result.Succeeded)
            throw new BusinessRuleViolationException(string.Join(", ", result.Errors.Select(e => e.Description)));

        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task UpdateRoleAsync(Guid userId, UpdateUserRoleRequest request, CancellationToken ct = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            throw new NotFoundException("User not found.");

        var currentRoles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, currentRoles.ToArray());

        var addResult = await _userManager.AddToRoleAsync(user, request.Role);
        if (!addResult.Succeeded)
            throw new BusinessRuleViolationException("Failed to update user role.");

        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task DeactivateAsync(Guid userId, CancellationToken ct = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            throw new NotFoundException("User not found.");

        user.IsActive = false;
        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            throw new BusinessRuleViolationException("Failed to deactivate user.");

        // Revoke all refresh tokens
        var tokens = await _refreshTokenRepository.GetAllAsync(ct);
        foreach (var token in tokens.Where(t => t.UserId == userId && !t.IsRevoked))
        {
            token.IsRevoked = true;
            _refreshTokenRepository.Update(token);
        }

        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task ActivateAsync(Guid userId, CancellationToken ct = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            throw new NotFoundException("User not found.");

        user.IsActive = true;
        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            throw new BusinessRuleViolationException("Failed to activate user.");

        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task<UserResponse> GetCurrentUserAsync(Guid userId, CancellationToken ct = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            throw new NotFoundException("User not found.");
        var userRoles = await _userManager.GetRolesAsync(user);
        return new UserResponse(user.Id, user.Email, user.FirstName, user.LastName, $"{user.FirstName} {user.LastName}", user.ProfilePictureUrl, user.IsActive, userRoles.FirstOrDefault() ?? "Developer", user.CreatedAt);
    }
}
