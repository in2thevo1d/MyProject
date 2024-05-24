using AutoMapper;
using MyProjectDomain.Entity;
using MyProjectDomain.Dto;

namespace MyProjectApplication.Mapping
{
    public class UserMapping : Profile
    {
        public UserMapping() 
        {
            CreateMap<User, UserDto>().ReverseMap();
        }
    }
}
