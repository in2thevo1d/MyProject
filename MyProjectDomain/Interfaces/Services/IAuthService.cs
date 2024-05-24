using MyProjectDomain.Dto;
using MyProjectDomain.Result;

namespace MyProjectDomain.Interfaces.Services
{
    public interface IAuthService
    {
        Task<BaseResult<UserDto>> Register(RegisterUserDto dto);
        Task<BaseResult<TokenDto>> Login(LoginUserDto dto);
    }
}
