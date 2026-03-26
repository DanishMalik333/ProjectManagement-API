using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using ProjectManagement.Core.DTOs.Attachments;
using ProjectManagement.Core.Entities;
using ProjectManagement.Core.Exceptions;
using ProjectManagement.Core.Interfaces;
using ProjectManagement.Services.Extensions;

namespace ProjectManagement.Services.Services;

public class AttachmentService : IAttachmentService
{
    private readonly IAttachmentRepository _attachmentRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUnitOfWork _unitOfWork;

    public AttachmentService(IAttachmentRepository attachmentRepository, UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork)
    {
        _attachmentRepository = attachmentRepository;
        _userManager = userManager;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<AttachmentResponse>> GetByTaskAsync(Guid taskId, CancellationToken ct = default)
    {
        var attachments = await _attachmentRepository.GetAllAsync(ct);
        var responses = new List<AttachmentResponse>();
        foreach (var a in attachments.Where(x => x.TaskId == taskId))
        {
            var uploader = await _userManager.FindByIdAsync(a.UploadedByUserId.ToString());
            responses.Add(new AttachmentResponse(a.Id, a.TaskId, a.UploadedByUserId, uploader?.FullName() ?? "Unknown", a.FileName, a.FileUrl, a.FileSizeBytes, a.ContentType, a.UploadedAt));
        }
        return responses;
    }

    public async Task<AttachmentResponse> UploadAsync(Guid taskId, IFormFile file, Guid uploaderId, CancellationToken ct = default)
    {
        var attachment = new TaskAttachment { TaskId = taskId, FileName = file.FileName, FileSizeBytes = file.Length, FileUrl = Guid.NewGuid().ToString(), ContentType = file.ContentType, UploadedByUserId = uploaderId };
        await _attachmentRepository.AddAsync(attachment, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        var uploader = await _userManager.FindByIdAsync(uploaderId.ToString());
        return new AttachmentResponse(attachment.Id, attachment.TaskId, attachment.UploadedByUserId, uploader?.FullName() ?? "Unknown", attachment.FileName, attachment.FileUrl, attachment.FileSizeBytes, attachment.ContentType, attachment.UploadedAt);
    }

    public async Task DeleteAsync(Guid taskId, Guid attachmentId, Guid currentUserId, CancellationToken ct = default)
    {
        var attachment = await _attachmentRepository.GetByIdAsync(attachmentId, ct);
        if (attachment == null) throw new NotFoundException("Attachment not found.");
        if (attachment.UploadedByUserId != currentUserId) throw new ForbiddenException("Can only delete own attachments.");
        _attachmentRepository.Delete(attachment);
        await _unitOfWork.SaveChangesAsync(ct);
    }
}
