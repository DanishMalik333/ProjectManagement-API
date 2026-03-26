namespace ProjectManagement.Core.DTOs.Comments;
public record CommentResponse(Guid Id, string Content, Guid TaskId, Guid AuthorId, string AuthorName, Guid? ParentCommentId, bool IsEdited, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, IEnumerable<CommentResponse>? Replies);
