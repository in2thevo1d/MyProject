using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MyProjectDomain.Entity;
using MyProjectDomain.Interfaces.Repositories;
using MyProjectDomain.Dto;
using MyProjectDomain.Interfaces.Services;
using MyProjectDomain.Result;
using MyProjectDomain.Settings;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace MyProjectApplication.Services
{
    public class TokenService : ITokenService
    {
        private readonly IBaseRepository<User> _userRepository;
        private readonly string _jwtKey;
        private readonly string _issuer;
        private readonly string _audience;

        public TokenService(IOptions<JwtSettings> options, IBaseRepository<User> userRepository)
        {
            _audience = options.Value.Audience;
            _issuer = options.Value.Issuer;
            _jwtKey = options.Value.JwtKey;
            _userRepository = userRepository;
        }

        public string GenerateRefreshToken()
        {
            var randomNumbers = new byte[32];
            using var randomNumberGenerator = RandomNumberGenerator.Create();
            randomNumberGenerator.GetBytes(randomNumbers);
            return Convert.ToBase64String(randomNumbers);
        }

        public string GenerateAccessToken(IEnumerable<Claim> claims)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var securityToken = new JwtSecurityToken(_issuer, _audience, claims, null, DateTime.UtcNow.AddMinutes(10), credentials);
            var token = new JwtSecurityTokenHandler().WriteToken(securityToken);
            return token;
        }

        public ClaimsPrincipal GetPrincipalFromExpiredRefreshToken(string accessToken)
        {
            var tokenValidationParametrs = new TokenValidationParameters()
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidAudience = _audience,
                ValidIssuer = _issuer,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey))
                
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var claimsPrincipal = tokenHandler.ValidateToken(accessToken, tokenValidationParametrs, out var securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }
            return claimsPrincipal;
        }

        public async Task<BaseResult<TokenDto>> RefreshToken(TokenDto dto)
        {
            var accessToken = dto.AccessToken;
            var refreshToken = dto.RefreshToken;

            var claimsPrincipal = GetPrincipalFromExpiredRefreshToken(accessToken);
            var userName = claimsPrincipal.Identity?.Name;

            var user = _userRepository.GetAll()
                .Include(x => x.UserToken)
                .FirstOrDefault(x => x.Login == userName);
            if (user == null || user.UserToken.RefreshToken != refreshToken || user.UserToken.RefreshTokenExpityTime <= DateTime.UtcNow)
            {
                return new BaseResult<TokenDto>()
                {
                    ErrorMessage = "Invalid client request"
                };
            }

            var newAccessToken = GenerateAccessToken(claimsPrincipal.Claims);
            var newRefreshToken = GenerateRefreshToken();

            user.UserToken.RefreshToken = newRefreshToken;
            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();

            return new BaseResult<TokenDto>()
            {
                Data = new TokenDto()
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken
                }
            };
        }
    }
}
