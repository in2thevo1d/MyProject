using MyProjectDomain.Entity;
using MyProjectDomain.Interfaces.Repositories;
using MyProjectDomain.Dto;
using MyProjectDomain.Enum;
using MyProjectDomain.Interfaces.Services;
using MyProjectDomain.Result;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using System.Security.Claims;

namespace MyProjectApplication.Services
{
    internal class AuthService : IAuthService
    {
        private readonly IBaseRepository<User> _userRepository;
        private readonly IBaseRepository<UserToken> _userTokenRepository;
        private readonly IBaseRepository<Role> _roleRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public AuthService(IBaseRepository<User> userRepository, IBaseRepository<UserToken> userTokenRepository, IBaseRepository<Role> roleRepository, IUnitOfWork unitOfWork, ITokenService tokenService, IMapper mapper)
        {
            _userRepository = userRepository;
            _userTokenRepository = userTokenRepository;
            _roleRepository = roleRepository;
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
            _mapper = mapper;
        }

        public async Task<BaseResult<UserDto>> Register(RegisterUserDto dto)
        {
            if (dto.Password != dto.ConfirmPassword)
            {
                return new BaseResult<UserDto>()
                {
                    ErrorMessage = "Passwords don't match",
                    ErrorCode = (int)ErrorCodes.PasswordsDontMatch
                };
            }
            var user = await _userRepository.GetAll().FirstOrDefaultAsync(x => x.Login == dto.Login);
            if (user != null)
            {
                return new BaseResult<UserDto>()
                {
                    ErrorMessage = "User already exist",
                    ErrorCode = (int)ErrorCodes.UserAlreadyExist
                };
            }
            var hashUserPassword = HashPassword(dto.Password);

            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    user = new User()
                    {
                        Login = dto.Login,
                        Password = hashUserPassword
                    };
                    await _unitOfWork.Users.CreateAsync(user);

                    await _unitOfWork.SaveChangesAsync();

                    var role = await _roleRepository.GetAll().FirstOrDefaultAsync(x => x.Name == nameof(Roles.User));
                    if (role == null)
                    {
                        return new BaseResult<UserDto>()
                        {
                            ErrorMessage = "Role not Found",
                            ErrorCode = (int)ErrorCodes.RoleNotFound
                        };
                    }

                    UserRole userRole = new UserRole()
                    {
                        UserId = user.Id,
                        RoleId = role.Id
                    };
                    await _unitOfWork.UserRoles.CreateAsync(userRole);

                    await _unitOfWork.SaveChangesAsync();

                    await transaction.CommitAsync();
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                }
            }

            return new BaseResult<UserDto>()
            {
                Data = _mapper.Map<UserDto>(user)
            };
        }

        public async Task<BaseResult<TokenDto>> Login(LoginUserDto dto)
        {
            var user = await _userRepository.GetAll()
                .Include(x => x.Roles)
                .FirstOrDefaultAsync(x => x.Login == dto.Login);
            if (user == null)
            {
                return new BaseResult<TokenDto>()
                {
                    ErrorMessage = "User not found",
                    ErrorCode = (int)ErrorCodes.UserNotFound
                };
            }

            if (!IsVerifyPassword(user.Password, dto.Password))
            {
                return new BaseResult<TokenDto>()
                {
                    ErrorMessage = "Wrong password",
                    ErrorCode = (int)ErrorCodes.WrongPassword
                };
            }
            var userToken = await _userTokenRepository.GetAll().FirstOrDefaultAsync(x => x.UserId == user.Id);

            var userRoles = user.Roles;
            var claims = userRoles.Select(x => new Claim(ClaimTypes.Role, x.Name)).ToList();
            claims.Add(new Claim(ClaimTypes.Name, user.Login));
            var accessToken = _tokenService.GenerateAccessToken(claims);
            var refreshToken = _tokenService.GenerateRefreshToken();

            if (userToken == null)
            {
                userToken = new UserToken()
                {
                    UserId = user.Id,
                    RefreshToken = refreshToken,
                    RefreshTokenExpityTime = DateTime.UtcNow.AddDays(7)
                };
                await _userTokenRepository.CreateAsync(userToken);
            }
            else
            {
                userToken.RefreshToken = refreshToken;
                userToken.RefreshTokenExpityTime = DateTime.UtcNow.AddDays(7);

                _userTokenRepository.Update(userToken);
                await _userTokenRepository.SaveChangesAsync();
            }

            return new BaseResult<TokenDto>()
            {
                Data = new TokenDto()
                {
                    RefreshToken = refreshToken,
                    AccessToken = accessToken
                }
            };
        }

        private string HashPassword(string password)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        private bool IsVerifyPassword(string userPasswordHash, string userPassword)
        {
            var hash = HashPassword(userPassword);
            return userPasswordHash == hash;
        }
    }
}