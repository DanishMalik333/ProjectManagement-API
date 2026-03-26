using ProjectManagement.Core.DTOs.Comments;
using ProjectManagement.Core.Entities;
using ProjectManagement.Core.Exceptions;
using ProjectManagement.Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using ProjectManagement.Services.Extensions;

namespace ProjectManagement.Services.Services;

public class CommentService : ICommentService
{
    private readonly ICommentRepository _commentRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUnitOfWork _unitOfWork;

    public CommentService(ICommentRepository commentRepository, UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork)
    {
        _commentRepository = commentRepository;
        _userManager = userManager;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<CommentResponse>> GetThreadedAsync(Guid taskId, Guid currentUserId, CancellationToken ct = default)
    {
        var comments = await _commentRepository.GetAllAsync(ct);
        var responses = new List<CommentResponse>();
        foreach (var c in comments.Where(x => x.TaskId == taskId && !x.ParentCommentId.HasValue && !x.IsDeleted))
        {
            var author = await _userManager.FindByIdAsync(c.AuthorId.ToString());
            responses.Add(new CommentResponse(c.Id, c.Content, c.TaskId, c.AuthorId, author?.FullName() ?? "Unknown", c.ParentCommentId, c.IsEdited, c.CreatedAt, c.UpdatedAt, new List<CommentResponse>()));
        }
        return responses;
    }

    public async Task<CommentResponse> CreateAsync(Guid taskId, CreateCommentRequest request, Guid authorId, CancellationToken ct = default)
    {
        var comment = new Comment { TaskId = taskId, Content = request.Content, AuthorId = authorId, ParentCommentId = request.ParentCommentId, IsEdited = false };
        await _commentRepository.AddAsync(comment, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        var author = await _userManager.FindByIdAsync(authorId.ToString());
        return new CommentResponse(comment.Id, comment.Content, comment.TaskId, comment.AuthorId, author?.FullName() ?? "Unknown", comment.ParentCommentId, comment.IsEdited, comment.CreatedAt, comment.UpdatedAt, new List<CommentResponse>());
    }

    public async Task<CommentResponse> UpdateAsync(Guid taskId, Guid commentId, UpdateCommentRequest request, Guid currentUserId, CancellationToken ct = default)
    {
        var comment = await _commentRepository.GetByIdAsync(commentId, ct);
        if (comment == null) throw new NotFoundException("Comment not found.");
        if (comment.AuthorId != currentUserId) throw new ForbiddenException("Can only update own comments.");
        comment.Content = request.Content;
        comment.IsEdited = true;
        _commentRepository.Update(comment);
        await _unitOfWork.SaveChangesAsync(ct);
        var author = await _userManager.FindByIdAsync(comment.AuthorId.ToString());
        return new CommentResponse(comment.Id, comment.Content, comment.TaskId, comment.AuthorId, author?.FullName() ?? "Unknown", comment.ParentCommentId, comment.IsEdited, comment.CreatedAt, comment.UpdatedAt, new List<CommentResponse>());
    }

    public async Task DeleteAsync(Guid taskId, Guid commentId, Guid currentUserId, CancellationToken ct = default)
    {
        var comment = await _commentRepository.GetByIdAsync(commentId, ct);
        if (comment == null) throw new NotFoundException("Comment not found.");
        if (comment.AuthorId != currentUserId) throw new ForbiddenException("Can only delete own comments.");
        comment.IsDeleted = true;
        _commentRepository.Update(comment);
        await _unitOfWork.SaveChangesAsync(ct);
    }
}
