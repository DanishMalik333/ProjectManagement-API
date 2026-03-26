using Microsoft.AspNetCore.Http;
using ProjectManagement.Core.DTOs.Attachments;
namespace ProjectManagement.Core.Interfaces;
public interface IAttachmentService
{
    Task<IEnumerable<AttachmentResponse>> GetByTaskAsync(Guid taskId, CancellationToken ct = default);
    Task<AttachmentResponse> UploadAsync(Guid taskId, IFormFile file, Guid uploaderId, CancellationToken ct = default);
    Task DeleteAsync(Guid taskId, Guid attachmentId, Guid currentUserId, CancellationToken ct = default);
}
