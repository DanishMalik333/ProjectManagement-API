using Microsoft.EntityFrameworkCore;
using ProjectManagement.Core.Entities;
using ProjectManagement.Core.Interfaces;
using ProjectManagement.Infrastructure.Persistence;

namespace ProjectManagement.Infrastructure.Repositories;

public class AttachmentRepository : GenericRepository<TaskAttachment>, IAttachmentRepository
{
    public AttachmentRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<TaskAttachment>> GetByTaskAsync(Guid taskId, CancellationToken ct = default) =>
        await _context.Attachments
            .Include(a => a.UploadedBy)
            .Where(a => a.TaskId == taskId)
            .ToListAsync(ct);
}
