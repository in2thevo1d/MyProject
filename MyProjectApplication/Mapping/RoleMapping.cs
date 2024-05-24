using AutoMapper;
using MyProjectDomain.Dto;
using MyProjectDomain.Entity;

namespace MyProjectApplication.Mapping
{
    public class RoleMapping : Profile
    {
        public RoleMapping()
        {
            CreateMap<Role, RoleDto>().ReverseMap();
        }
    }
}
