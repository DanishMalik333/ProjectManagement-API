using ProjectManagement.Core.Entities;
namespace ProjectManagement.Core.Interfaces;
public interface IAttachmentRepository : IRepository<TaskAttachment>
{
    Task<IEnumerable<TaskAttachment>> GetByTaskAsync(Guid taskId, CancellationToken ct);
}
