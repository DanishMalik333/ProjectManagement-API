using AutoMapper;
using ProjectManagement.Core.DTOs.Tasks;
using ProjectManagement.Core.Entities;

namespace ProjectManagement.Services.Mappings;

public class TaskProfile : Profile
{
    public TaskProfile()
    {
        CreateMap<ProjectTask, TaskResponse>();
        CreateMap<CreateTaskRequest, ProjectTask>();
        CreateMap<UpdateTaskRequest, ProjectTask>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<TaskHistory, TaskHistoryResponse>()
            .ForMember(dest => dest.ChangedByName, opt => opt.MapFrom(src => src.ChangedBy != null ? $"{src.ChangedBy.FirstName} {src.ChangedBy.LastName}".Trim() : "Unknown"));
    }
}
