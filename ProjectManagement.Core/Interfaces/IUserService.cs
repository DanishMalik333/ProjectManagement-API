using ProjectManagement.Core.DTOs.Common;
using ProjectManagement.Core.DTOs.Users;
using ProjectManagement.Core.DTOs.Auth;
namespace ProjectManagement.Core.Interfaces;
public interface IUserService
{
    Task<PagedResult<UserResponse>> GetAllAsync(string? role, bool? isActive, string? search, int page, int pageSize, CancellationToken ct = default);
    Task<UserResponse> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task UpdateProfileAsync(Guid userId, UpdateProfileRequest request, CancellationToken ct = default);
    Task ChangePasswordAsync(Guid userId, ChangePasswordRequest request, CancellationToken ct = default);
    Task UpdateRoleAsync(Guid userId, UpdateUserRoleRequest request, CancellationToken ct = default);
    Task DeactivateAsync(Guid userId, CancellationToken ct = default);
    Task ActivateAsync(Guid userId, CancellationToken ct = default);
    Task<UserResponse> GetCurrentUserAsync(Guid userId, CancellationToken ct = default);
}
