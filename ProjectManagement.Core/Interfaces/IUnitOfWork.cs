namespace ProjectManagement.Core.Interfaces;
public interface IUnitOfWork : IDisposable
{
    ITeamRepository Teams { get; }
    IProjectRepository Projects { get; }
    ISprintRepository Sprints { get; }
    ITaskRepository Tasks { get; }
    ICommentRepository Comments { get; }
    ILabelRepository Labels { get; }
    INotificationRepository Notifications { get; }
    ITaskHistoryRepository TaskHistories { get; }
    IAttachmentRepository Attachments { get; }
    IRefreshTokenRepository RefreshTokens { get; }
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
