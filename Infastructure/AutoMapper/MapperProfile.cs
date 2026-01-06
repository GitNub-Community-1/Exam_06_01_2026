using AutoMapper;
using Domain.Dtos;


namespace Infastructure.AutoMapper;


public class MapperProfile : Profile
{
    public MapperProfile()
    {
        CreateMap<UserActivityDto,UserActivity>().ReverseMap();
        CreateMap<UserActivityCreatDto,UserActivity>().ReverseMap();
        CreateMap<ActivityTypeCreatDto,ActivityType>().ReverseMap();
        CreateMap<ActivityTypeDto,ActivityType>().ReverseMap();
        CreateMap<UserDto,User>().ReverseMap();
    }
}