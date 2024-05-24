using MyProjectDomain.Dto;
using MyProjectDomain.Result;

namespace MyProjectDomain.Interfaces.Services
{
    public interface IRoleService
    {
        Task<BaseResult<RoleDto>> CreateRoleAsync(CreateRoleDto dto);
        Task<BaseResult<RoleDto>> DeleteRoleAsync(long Id);
        Task<BaseResult<RoleDto>> UpdateRoleAsync(RoleDto dto);
        Task<BaseResult<UserRoleDto>> AddRoleForUserAsync(UserRoleDto dto);
        Task<BaseResult<UserRoleDto>> DeleteRoleForUserAsync(DeleteUserRoleDto dto);
        Task<BaseResult<UserRoleDto>> UpdateRoleForUserAsync (UpdateUserRoleDto dto);
    }
}
