using AutoMapper;
using ProjectManagement.Core.DTOs.Attachments;
using ProjectManagement.Core.Entities;

namespace ProjectManagement.Services.Mappings;

public class AttachmentProfile : Profile
{
    public AttachmentProfile()
    {
        CreateMap<TaskAttachment, AttachmentResponse>()
            .ForMember(dest => dest.UploadedByName, opt => opt.MapFrom(src => src.UploadedBy != null ? $"{src.UploadedBy.FirstName} {src.UploadedBy.LastName}".Trim() : "Unknown"));
    }
}
