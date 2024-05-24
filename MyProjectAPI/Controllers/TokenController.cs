using Microsoft.AspNetCore.Mvc;
using MyProjectDomain.Dto;
using MyProjectDomain.Interfaces.Services;
using MyProjectDomain.Result;

namespace MyProjectAPI.Controllers
{
    [ApiController]
    public class TokenController : Controller
    {
        private readonly ITokenService _tokenService;

        public TokenController(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        [HttpPost]
        [Route("refresh")]
        public async Task<ActionResult<BaseResult<TokenDto>>> RefreshToken([FromBody] TokenDto tokenDto)
        {
            var responce = await _tokenService.RefreshToken(tokenDto);
            if (responce.IsSuccess)
            {
                return Ok(responce);
            }
            return BadRequest(responce);
        }
    }
}