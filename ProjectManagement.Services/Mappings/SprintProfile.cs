using AutoMapper;
using ProjectManagement.Core.DTOs.Sprints;
using ProjectManagement.Core.Entities;

namespace ProjectManagement.Services.Mappings;

public class SprintProfile : Profile
{
    public SprintProfile()
    {
        CreateMap<Sprint, SprintResponse>();
        CreateMap<CreateSprintRequest, Sprint>();
        CreateMap<UpdateSprintRequest, Sprint>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}
