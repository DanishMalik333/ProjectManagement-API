using AutoMapper;
using ProjectManagement.Core.DTOs.Teams;
using ProjectManagement.Core.Entities;

namespace ProjectManagement.Services.Mappings;

public class TeamProfile : Profile
{
    public TeamProfile()
    {
        CreateMap<Team, TeamResponse>()
            .ForMember(dest => dest.CreatedByName, opt => opt.MapFrom(src => src.CreatedBy != null ? $"{src.CreatedBy.FirstName} {src.CreatedBy.LastName}".Trim() : "Unknown"));

        CreateMap<CreateTeamRequest, Team>();
        CreateMap<UpdateTeamRequest, Team>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<TeamMember, TeamMemberResponse>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User != null ? $"{src.User.FirstName} {src.User.LastName}".Trim() : "Unknown"));
    }
}
