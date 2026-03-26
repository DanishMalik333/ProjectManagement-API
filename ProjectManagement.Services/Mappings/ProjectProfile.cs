using AutoMapper;
using ProjectManagement.Core.DTOs.Projects;
using ProjectManagement.Core.Entities;

namespace ProjectManagement.Services.Mappings;

public class ProjectProfile : Profile
{
    public ProjectProfile()
    {
        CreateMap<Project, ProjectResponse>()
            .ForMember(dest => dest.OwnerName, opt => opt.MapFrom(src => src.Owner != null ? $"{src.Owner.FirstName} {src.Owner.LastName}".Trim() : "Unknown"));

        CreateMap<CreateProjectRequest, Project>();
        CreateMap<UpdateProjectRequest, Project>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<Label, LabelResponse>();
        CreateMap<CreateLabelRequest, Label>();
    }
}
