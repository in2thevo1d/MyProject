using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MyProjectDomain.Interfaces.Repositories;
using MyProjectDomain.Dto;
using MyProjectDomain.Entity;
using MyProjectDomain.Enum;
using MyProjectDomain.Interfaces.Services;
using MyProjectDomain.Result;

namespace MyProjectApplication.Services
{
    public class RoleService : IRoleService
    {
        private readonly IBaseRepository<User> _userRepository;
        private readonly IBaseRepository<Role> _roleRepository;
        private readonly IBaseRepository<UserRole> _userRoleRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public RoleService(IBaseRepository<User> userRepository, IBaseRepository<Role> roleRepository, IBaseRepository<UserRole> userRoleRepository, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        } 

        public async Task<BaseResult<RoleDto>> CreateRoleAsync(CreateRoleDto dto)
        {
            var role = await _roleRepository.GetAll().FirstOrDefaultAsync(x => x.Name == dto.Name);
            if (role != null)
            {
                return new BaseResult<RoleDto>()
                {
                    ErrorMessage = "Role already exist",
                    ErrorCode = (int)ErrorCodes.RoleAlreadyExist
                };
            }

            role = new Role()
            {
                Name = dto.Name,
            };
            await _roleRepository.CreateAsync(role);

            return new BaseResult<RoleDto>()
            {
                Data = _mapper.Map<RoleDto>(role),
            };
        }

        public async Task<BaseResult<RoleDto>> DeleteRoleAsync(long Id)
        {
            var role = await _roleRepository.GetAll().FirstOrDefaultAsync(x => x.Id == Id);
            if (role == null)
            {
                return new BaseResult<RoleDto>()
                {
                    ErrorMessage = "Role not found",
                    ErrorCode = (int)ErrorCodes.RoleNotFound
                };
            }
            _roleRepository.Remove(role);
            await _roleRepository.SaveChangesAsync();

            return new BaseResult<RoleDto>()
            {
                Data = _mapper.Map<RoleDto>(role),
            };
        }

        public async Task<BaseResult<RoleDto>> UpdateRoleAsync(RoleDto dto)
        {
            var role = await _roleRepository.GetAll().FirstOrDefaultAsync(x => x.Id == dto.Id);
            if (role == null)
            {
                return new BaseResult<RoleDto>()
                {
                    ErrorMessage = "Role not found",
                    ErrorCode = (int)ErrorCodes.RoleNotFound
                };
            }

            role.Name = dto.Name;
            var updatedRole = _roleRepository.Update(role);
            await _roleRepository.SaveChangesAsync();

            return new BaseResult<RoleDto>()
            {
                Data = _mapper.Map<RoleDto>(updatedRole),
            };
        }

        public async Task<BaseResult<UserRoleDto>> AddRoleForUserAsync(UserRoleDto dto)
        {
            var user = await _userRepository.GetAll()
                .Include(x => x.Roles)
                .FirstOrDefaultAsync(x => x.Login == dto.Login);

            if (user == null)
            {
                return new BaseResult<UserRoleDto>
                {
                    ErrorMessage = "User not found",
                    ErrorCode = (int)ErrorCodes.UserNotFound
                };
            }

            var roles = user.Roles.Select(x => x.Name).ToArray();
            if (roles.Any(x => x != dto.RoleName))
            {
                var role = await _roleRepository.GetAll().FirstOrDefaultAsync(x => x.Name == dto.RoleName);
                if (role == null)
                {
                    return new BaseResult<UserRoleDto>
                    {
                        ErrorMessage = "Role not found",
                        ErrorCode = (int)ErrorCodes.RoleNotFound
                    };
                }

                UserRole userRole = new UserRole()
                {
                    RoleId = role.Id,
                    UserId = user.Id
                };
                await _userRoleRepository.CreateAsync(userRole);

                return new BaseResult<UserRoleDto>
                {
                    Data = new UserRoleDto()
                    {
                        Login = user.Login,
                        RoleName = role.Name,
                    }
                };
            }
            return new BaseResult<UserRoleDto>
            {
                ErrorMessage = "User already exist this role",
                ErrorCode = (int)ErrorCodes.UserAlreadyExistThisRole
            };
        }

        public async Task<BaseResult<UserRoleDto>> DeleteRoleForUserAsync(DeleteUserRoleDto dto)
        {
            var user = await _userRepository.GetAll()
                .Include(x => x.Roles)
                .FirstOrDefaultAsync(x => x.Login == dto.Login);

            if (user == null)
            {
                return new BaseResult<UserRoleDto>
                {
                    ErrorMessage = "User not found",
                    ErrorCode = (int)ErrorCodes.UserNotFound
                };
            }
            var role = user.Roles.FirstOrDefault(x => x.Id == dto.RoleId);
            if (role == null)
            {
                return new BaseResult<UserRoleDto>
                {
                    ErrorMessage = "Role not found",
                    ErrorCode = (int)ErrorCodes.RoleNotFound
                };
            }
            var userRole = await _userRoleRepository.GetAll()
                .Where(x => x.RoleId == role.Id)
                .FirstOrDefaultAsync(x => x.UserId == user.Id);
            _userRoleRepository.Remove(userRole);
            await _userRepository.SaveChangesAsync();

            return new BaseResult<UserRoleDto>
            {
                Data = new UserRoleDto()
                {
                    Login = user.Login,
                    RoleName = role.Name,
                }
            };
        }

        public async Task<BaseResult<UserRoleDto>> UpdateRoleForUserAsync(UpdateUserRoleDto dto)
        {
            var user = await _userRepository.GetAll()
                .Include(x => x.Roles)
                .FirstOrDefaultAsync(x => x.Login == dto.Login);
            if (user == null)
            {
                return new BaseResult<UserRoleDto>
                {
                    ErrorMessage = "User not found",
                    ErrorCode = (int)ErrorCodes.UserNotFound
                };
            }
            var role = user.Roles.FirstOrDefault(x => x.Id == dto.FromRoleId);
            if (role == null)
            {
                return new BaseResult<UserRoleDto>
                {
                    ErrorMessage = "Role not found",
                    ErrorCode = (int)ErrorCodes.RoleNotFound
                };
            }

            var newRoleForUser = await _roleRepository.GetAll().FirstOrDefaultAsync(x => x.Id == dto.ToRoleId);
            if (newRoleForUser == null)
            {
                return new BaseResult<UserRoleDto>
                {
                    ErrorMessage = "Role not found",
                    ErrorCode = (int)ErrorCodes.RoleNotFound
                };
            }

            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var userRole = await _unitOfWork.UserRoles.GetAll()
                    .Where(x => x.RoleId == role.Id)
                    .FirstOrDefaultAsync(x => x.UserId == user.Id);

                    _unitOfWork.UserRoles.Remove(userRole);
                    await _unitOfWork.SaveChangesAsync();

                    var newUserRole = new UserRole()
                    {
                        UserId = user.Id,
                        RoleId = newRoleForUser.Id,
                    };
                    await _unitOfWork.UserRoles.CreateAsync(newUserRole);
                    await _unitOfWork.SaveChangesAsync();

                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                }
            }

            return new BaseResult<UserRoleDto>
            {
                Data = new UserRoleDto()
                {
                    Login = user.Login,
                    RoleName = newRoleForUser.Name
                }
            };
        }
    }
}