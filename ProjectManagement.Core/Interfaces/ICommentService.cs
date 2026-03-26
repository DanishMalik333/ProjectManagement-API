using ProjectManagement.Core.DTOs.Comments;
namespace ProjectManagement.Core.Interfaces;
public interface ICommentService
{
    Task<IEnumerable<CommentResponse>> GetThreadedAsync(Guid taskId, Guid currentUserId, CancellationToken ct = default);
    Task<CommentResponse> CreateAsync(Guid taskId, CreateCommentRequest request, Guid authorId, CancellationToken ct = default);
    Task<CommentResponse> UpdateAsync(Guid taskId, Guid commentId, UpdateCommentRequest request, Guid currentUserId, CancellationToken ct = default);
    Task DeleteAsync(Guid taskId, Guid commentId, Guid currentUserId, CancellationToken ct = default);
}
