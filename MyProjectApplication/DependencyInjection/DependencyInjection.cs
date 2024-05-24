using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using MyProjectApplication.Mapping;
using MyProjectApplication.Services;
using MyProjectApplication.Validations;
using MyProjectApplication.Validations.FluentValidations;
using MyProjectDomain.Dto;
using MyProjectDomain.Interfaces.Services;
using MyProjectDomain.Validations;

namespace MyProjectApplication.DependencyInjection
{
    public static class DependencyInjection
    {
        public static void AddApplication(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(ReportMapping));

            InitServices(services);
        }

        private static void InitServices(this IServiceCollection services)
        {
            services.AddScoped<IReportValidator, ReportValidator>();
            services.AddScoped<IValidator<CreateReportDto>, CreateReportValidator>();
            services.AddScoped<IValidator<UpdateReportDto>, UpdateReportValidator>();

            services.AddScoped<IReportService, ReportService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ITokenService, TokenService>();
        }
    }
}
