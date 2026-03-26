namespace ProjectManagement.Core.DTOs.Comments;
public record CreateCommentRequest(string Content, Guid? ParentCommentId);
