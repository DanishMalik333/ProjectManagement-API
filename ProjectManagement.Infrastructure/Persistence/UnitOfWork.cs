using ProjectManagement.Core.Interfaces;
using ProjectManagement.Infrastructure.Repositories;

namespace ProjectManagement.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private ITeamRepository? _teams;
    private IProjectRepository? _projects;
    private ISprintRepository? _sprints;
    private ITaskRepository? _tasks;
    private ICommentRepository? _comments;
    private ILabelRepository? _labels;
    private INotificationRepository? _notifications;
    private ITaskHistoryRepository? _taskHistories;
    private IAttachmentRepository? _attachments;
    private IRefreshTokenRepository? _refreshTokens;

    public UnitOfWork(AppDbContext context) { _context = context; }

    public ITeamRepository Teams => _teams ??= new TeamRepository(_context);
    public IProjectRepository Projects => _projects ??= new ProjectRepository(_context);
    public ISprintRepository Sprints => _sprints ??= new SprintRepository(_context);
    public ITaskRepository Tasks => _tasks ??= new TaskRepository(_context);
    public ICommentRepository Comments => _comments ??= new CommentRepository(_context);
    public ILabelRepository Labels => _labels ??= new LabelRepository(_context);
    public INotificationRepository Notifications => _notifications ??= new NotificationRepository(_context);
    public ITaskHistoryRepository TaskHistories => _taskHistories ??= new TaskHistoryRepository(_context);
    public IAttachmentRepository Attachments => _attachments ??= new AttachmentRepository(_context);
    public IRefreshTokenRepository RefreshTokens => _refreshTokens ??= new RefreshTokenRepository(_context);

    public async Task<int> SaveChangesAsync(CancellationToken ct = default) =>
        await _context.SaveChangesAsync(ct);

    public void Dispose() => _context.Dispose();
}
