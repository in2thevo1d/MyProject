using Microsoft.EntityFrameworkCore.Storage;
using MyProjectDomain.Entity;

namespace MyProjectDomain.Interfaces.Repositories
{
    public interface IUnitOfWork : IStateSaveChanges
    {
        Task<IDbContextTransaction> BeginTransactionAsync();
        IBaseRepository<User> Users { get; set; }
        IBaseRepository<Role> Roles { get; set; }
        IBaseRepository<UserRole> UserRoles { get; set; }
    }
}
