using ProjectManagement.Core.Entities;
namespace ProjectManagement.Core.Interfaces;
public interface ICommentRepository : IRepository<Comment>
{
    Task<IEnumerable<Comment>> GetThreadedAsync(Guid taskId, CancellationToken ct);
}
