using AutoMapper;
using ProjectManagement.Core.DTOs.Notifications;
using ProjectManagement.Core.Entities;

namespace ProjectManagement.Services.Mappings;

public class NotificationProfile : Profile
{
    public NotificationProfile()
    {
        CreateMap<Notification, NotificationResponse>();
    }
}
