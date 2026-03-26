using ProjectManagement.Core.DTOs.Common;
using ProjectManagement.Core.DTOs.Projects;
using ProjectManagement.Core.Entities;
using ProjectManagement.Core.Exceptions;
using ProjectManagement.Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using ProjectManagement.Services.Extensions;

namespace ProjectManagement.Services.Services;

public class ProjectService : IProjectService
{
    private readonly IProjectRepository _projectRepository;
    private readonly ISprintRepository _sprintRepository;
    private readonly ITaskRepository _taskRepository;
    private readonly ILabelRepository _labelRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager;

    public ProjectService(
        IProjectRepository projectRepository,
        ISprintRepository sprintRepository,
        ITaskRepository taskRepository,
        ILabelRepository labelRepository,
        IUnitOfWork unitOfWork,
        UserManager<ApplicationUser> userManager)
    {
        _projectRepository = projectRepository;
        _sprintRepository = sprintRepository;
        _taskRepository = taskRepository;
        _labelRepository = labelRepository;
        _unitOfWork = unitOfWork;
        _userManager = userManager;
    }

    public async Task<PagedResult<ProjectResponse>> GetAccessibleAsync(Guid userId, string? teamId, string? status, int page, int pageSize, CancellationToken ct = default)
    {
        var projects = await _projectRepository.GetAllAsync(ct);
        var paged = projects.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        var responses = new List<ProjectResponse>();
        foreach (var p in paged)
        {
            var owner = await _userManager.FindByIdAsync(p.OwnerId.ToString());
            responses.Add(new ProjectResponse(p.Id, p.Name, p.Description, p.Key, p.Status.ToString(), p.TeamId, "Team", p.OwnerId, owner?.FullName() ?? "Unknown", null, null, p.CreatedAt, p.UpdatedAt));
        }
        var totalPages = (projects.Count() + pageSize - 1) / pageSize;
        return new PagedResult<ProjectResponse>(responses, projects.Count(), page, pageSize, totalPages);
    }

    public async Task<ProjectResponse> CreateAsync(CreateProjectRequest request, Guid creatorId, CancellationToken ct = default)
    {
        var project = new Project { Name = request.Name, Key = request.Key, Description = request.Description, TeamId = Guid.Empty, OwnerId = creatorId, Status = Core.Enums.ProjectStatus.Active };
        await _projectRepository.AddAsync(project, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        var owner = await _userManager.FindByIdAsync(creatorId.ToString());
        return new ProjectResponse(project.Id, project.Name, project.Description, project.Key, project.Status.ToString(), project.TeamId, "Team", project.OwnerId, owner?.FullName() ?? "Unknown", null, null, project.CreatedAt, project.UpdatedAt);
    }

    public async Task<ProjectResponse> GetByIdAsync(Guid id, Guid currentUserId, CancellationToken ct = default)
    {
        var project = await _projectRepository.GetByIdAsync(id, ct);
        if (project == null) throw new NotFoundException("Project not found.");
        var owner = await _userManager.FindByIdAsync(project.OwnerId.ToString());
        return new ProjectResponse(project.Id, project.Name, project.Description, project.Key, project.Status.ToString(), project.TeamId, "Team", project.OwnerId, owner?.FullName() ?? "Unknown", null, null, project.CreatedAt, project.UpdatedAt);
    }

    public async Task<ProjectResponse> UpdateAsync(Guid id, UpdateProjectRequest request, Guid currentUserId, CancellationToken ct = default)
    {
        var project = await _projectRepository.GetByIdAsync(id, ct);
        if (project == null) throw new NotFoundException("Project not found.");
        project.Name = request.Name;
        project.Description = request.Description;
        _projectRepository.Update(project);
        await _unitOfWork.SaveChangesAsync(ct);
        var owner = await _userManager.FindByIdAsync(project.OwnerId.ToString());
        return new ProjectResponse(project.Id, project.Name, project.Description, project.Key, project.Status.ToString(), project.TeamId, "Team", project.OwnerId, owner?.FullName() ?? "Unknown", null, null, project.CreatedAt, project.UpdatedAt);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var project = await _projectRepository.GetByIdAsync(id, ct);
        if (project == null) throw new NotFoundException("Project not found.");
        _projectRepository.Delete(project);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task<ProjectStatsResponse> GetStatsAsync(Guid id, Guid currentUserId, CancellationToken ct = default)
    {
        var tasks = await _taskRepository.GetAllAsync(ct);
        var pTasks = tasks.Where(t => t.ProjectId == id).ToList();
        var byStatus = pTasks.GroupBy(t => t.Status).ToDictionary(x => x.Key.ToString(), x => x.Count());
        var byPriority = pTasks.GroupBy(t => t.Priority).ToDictionary(x => x.Key.ToString(), x => x.Count());
        return new ProjectStatsResponse(byStatus, byPriority, 0, null);
    }

    public async Task<IEnumerable<LabelResponse>> GetLabelsAsync(Guid projectId, Guid currentUserId, CancellationToken ct = default)
    {
        var labels = await _labelRepository.GetAllAsync(ct);
        return labels.Where(l => l.ProjectId == projectId).Select(l => new LabelResponse(l.Id, l.Name, l.Color, l.ProjectId));
    }

    public async Task<LabelResponse> CreateLabelAsync(Guid projectId, CreateLabelRequest request, CancellationToken ct = default)
    {
        var label = new Label { ProjectId = projectId, Name = request.Name, Color = request.Color };
        await _labelRepository.AddAsync(label, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return new LabelResponse(label.Id, label.Name, label.Color, label.ProjectId);
    }

    public async Task<LabelResponse> UpdateLabelAsync(Guid projectId, Guid labelId, UpdateLabelRequest request, CancellationToken ct = default)
    {
        var label = await _labelRepository.GetByIdAsync(labelId, ct);
        if (label == null) throw new NotFoundException("Label not found.");
        label.Name = request.Name;
        label.Color = request.Color;
        _labelRepository.Update(label);
        await _unitOfWork.SaveChangesAsync(ct);
        return new LabelResponse(label.Id, label.Name, label.Color, label.ProjectId);
    }

    public async Task DeleteLabelAsync(Guid projectId, Guid labelId, CancellationToken ct = default)
    {
        var label = await _labelRepository.GetByIdAsync(labelId, ct);
        if (label == null) throw new NotFoundException("Label not found.");
        _labelRepository.Delete(label);
        await _unitOfWork.SaveChangesAsync(ct);
    }
}
