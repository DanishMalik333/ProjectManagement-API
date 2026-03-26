using AutoMapper;
using ProjectManagement.Core.DTOs.Auth;
using ProjectManagement.Core.DTOs.Users;
using ProjectManagement.Core.Entities;

namespace ProjectManagement.Services.Mappings;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<ApplicationUser, UserResponse>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}".Trim()))
            .ForMember(dest => dest.Role, opt => opt.Ignore());

        CreateMap<RegisterRequest, ApplicationUser>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email));
    }
}
