using Microsoft.AspNetCore.Mvc;
using MyProjectDomain.Dto;
using MyProjectDomain.Interfaces.Services;
using MyProjectDomain.Result;

namespace MyProjectAPI.Controllers
{
    [ApiController]
    public class AuthorizationController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthorizationController(IAuthService authService) 
        {
            _authService = authService;
        }

        [HttpPost("Registration")]
        public async Task<ActionResult<BaseResult<UserDto>>> Registration([FromBody] RegisterUserDto dto)
        {
            var responce = await _authService.Register(dto);
            if (responce.IsSuccess)
            {
                return Ok(responce);
            }
            return BadRequest(responce);
        }

        [HttpPost("Login")]
        public async Task<ActionResult<BaseResult<TokenDto>>> Login([FromBody] LoginUserDto dto)
        {
            var responce = await _authService.Login(dto);
            if (responce.IsSuccess)
            {
                return Ok(responce);
            }
            return BadRequest(responce);
        }
    }
}
