using MyProjectDomain.Dto;
using MyProjectDomain.Result;
using System.Security.Claims;

namespace MyProjectDomain.Interfaces.Services
{
    public interface ITokenService
    {
        string GenerateAccessToken(IEnumerable<Claim> claims);
        string GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredRefreshToken(string accessToken);
        Task<BaseResult<TokenDto>> RefreshToken(TokenDto dto);
    }
}