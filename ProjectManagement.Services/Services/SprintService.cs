using ProjectManagement.Core.DTOs.Sprints;
using ProjectManagement.Core.Entities;
using ProjectManagement.Core.Enums;
using ProjectManagement.Core.Exceptions;
using ProjectManagement.Core.Interfaces;

namespace ProjectManagement.Services.Services;

public class SprintService : ISprintService
{
    private readonly ISprintRepository _sprintRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly ITaskRepository _taskRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SprintService(ISprintRepository sprintRepository, IProjectRepository projectRepository, ITaskRepository taskRepository, IUnitOfWork unitOfWork)
    {
        _sprintRepository = sprintRepository;
        _projectRepository = projectRepository;
        _taskRepository = taskRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<SprintResponse>> GetByProjectAsync(Guid projectId, string? status, Guid currentUserId, CancellationToken ct = default)
    {
        var sprints = await _sprintRepository.GetAllAsync(ct);
        return sprints.Where(s => s.ProjectId == projectId).Select(s => new SprintResponse(s.Id, s.Name, s.Goal, s.ProjectId, s.Status.ToString(), s.StartDate, s.EndDate, s.CreatedAt, s.UpdatedAt, s.Tasks?.Count));
    }

    public async Task<SprintResponse> CreateAsync(Guid projectId, CreateSprintRequest request, Guid currentUserId, CancellationToken ct = default)
    {
        var sprint = new Sprint { ProjectId = projectId, Name = request.Name, Goal = request.Goal, StartDate = request.StartDate, EndDate = request.EndDate, Status = SprintStatus.Planning };
        await _sprintRepository.AddAsync(sprint, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return new SprintResponse(sprint.Id, sprint.Name, sprint.Goal, sprint.ProjectId, sprint.Status.ToString(), sprint.StartDate, sprint.EndDate, sprint.CreatedAt, sprint.UpdatedAt, sprint.Tasks?.Count);
    }

    public async Task<SprintResponse> GetByIdAsync(Guid projectId, Guid sprintId, Guid currentUserId, CancellationToken ct = default)
    {
        var sprint = await _sprintRepository.GetByIdAsync(sprintId, ct);
        if (sprint == null) throw new NotFoundException("Sprint not found.");
        return new SprintResponse(sprint.Id, sprint.Name, sprint.Goal, sprint.ProjectId, sprint.Status.ToString(), sprint.StartDate, sprint.EndDate, sprint.CreatedAt, sprint.UpdatedAt, sprint.Tasks?.Count);
    }

    public async Task<SprintResponse> UpdateAsync(Guid projectId, Guid sprintId, UpdateSprintRequest request, Guid currentUserId, CancellationToken ct = default)
    {
        var sprint = await _sprintRepository.GetByIdAsync(sprintId, ct);
        if (sprint == null) throw new NotFoundException("Sprint not found.");
        sprint.Name = request.Name;
        sprint.Goal = request.Goal;
        _sprintRepository.Update(sprint);
        await _unitOfWork.SaveChangesAsync(ct);
        return new SprintResponse(sprint.Id, sprint.Name, sprint.Goal, sprint.ProjectId, sprint.Status.ToString(), sprint.StartDate, sprint.EndDate, sprint.CreatedAt, sprint.UpdatedAt, sprint.Tasks?.Count);
    }

    public async Task<SprintResponse> StartAsync(Guid projectId, Guid sprintId, Guid currentUserId, CancellationToken ct = default)
    {
        var sprint = await _sprintRepository.GetByIdAsync(sprintId, ct);
        if (sprint == null) throw new NotFoundException("Sprint not found.");
        sprint.Status = SprintStatus.Active;
        _sprintRepository.Update(sprint);
        await _unitOfWork.SaveChangesAsync(ct);
        return new SprintResponse(sprint.Id, sprint.Name, sprint.Goal, sprint.ProjectId, sprint.Status.ToString(), sprint.StartDate, sprint.EndDate, sprint.CreatedAt, sprint.UpdatedAt, sprint.Tasks?.Count);
    }

    public async Task<SprintResponse> CompleteAsync(Guid projectId, Guid sprintId, CompleteSprintRequest request, Guid currentUserId, CancellationToken ct = default)
    {
        var sprint = await _sprintRepository.GetByIdAsync(sprintId, ct);
        if (sprint == null) throw new NotFoundException("Sprint not found.");
        sprint.Status = SprintStatus.Completed;
        _sprintRepository.Update(sprint);
        await _unitOfWork.SaveChangesAsync(ct);
        return new SprintResponse(sprint.Id, sprint.Name, sprint.Goal, sprint.ProjectId, sprint.Status.ToString(), sprint.StartDate, sprint.EndDate, sprint.CreatedAt, sprint.UpdatedAt, sprint.Tasks?.Count);
    }

    public async Task DeleteAsync(Guid projectId, Guid sprintId, Guid currentUserId, CancellationToken ct = default)
    {
        var sprint = await _sprintRepository.GetByIdAsync(sprintId, ct);
        if (sprint == null) throw new NotFoundException("Sprint not found.");
        _sprintRepository.Delete(sprint);
        await _unitOfWork.SaveChangesAsync(ct);
    }
}
