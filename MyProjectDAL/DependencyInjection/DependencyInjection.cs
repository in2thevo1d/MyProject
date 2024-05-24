using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyProjectDAL.Interceptors;
using MyProjectDAL.Repositories;
using MyProjectDomain.Entity;
using MyProjectDomain.Interfaces.Repositories;

namespace MyProjectDAL.DependencyInjection
{
    public static class DependencyInjection 
    {
        public static void AddDataAccessLayer(this IServiceCollection services, IConfiguration consfiguration)
        {
            var connectionString = consfiguration.GetConnectionString("PostgresSQL");

            services.AddSingleton<DateInterceptor>();
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(connectionString);
            });
            services.InitRepositories();
        }

        private static void InitRepositories(this IServiceCollection services)
        {
            services.AddScoped<IBaseRepository<User>, BaseRepository<User>>();
            services.AddScoped<IBaseRepository<Role>, BaseRepository<Role>>();
            services.AddScoped<IBaseRepository<UserRole>, BaseRepository<UserRole>>();
            services.AddScoped<IBaseRepository<UserToken>, BaseRepository<UserToken>>();
            services.AddScoped<IBaseRepository<Report>, BaseRepository<Report>>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }
    }
}
