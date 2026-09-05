
using HRMS.Application.Services;
using MediStock360.Application.Common;
using MediStock360.Application.Interfaces;
using MediStock360.Application.Services;
using MediStock360.Domain.Interfaces;
using MediStock360.Infrastructure.Interfaces;
using MediStock360.Infrastructure.Repositories;

namespace MediStock360.API.Extensions
{
    public static class RegisterServicesExtension
    {
        public static void RegisterService(IServiceCollection services, IConfiguration configuration)
        {
            #region RegisterAllService
            //services.AddScoped<IUtilityService, UtilityService>();
            services.AddScoped<IAuthService, AuthService>();
            //services.AddScoped<IClientService, ClientService>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            //services.AddScoped<IUserService, UserService>();
            //services.AddScoped<IUtilityService, UtilityService>();
            services.AddScoped<IRedisCacheService, RedisCacheService>();
            services.AddScoped<ISettingService, SettingService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IOTPService, OTPService>();
            services.AddScoped<IPermissionService, PermissionService>();
            services.AddScoped<IMenuService, MenuService>();
            //services.AddScoped<IMasterDataService, MasterDataService>();
            //services.AddScoped<IDepartmentService, DepartmentService>();
            //services.AddScoped<IDesignationService, DesignationService>();
            //services.AddScoped<IEmployeeService, EmployeeService>();
            //services.AddScoped<ILeaveService, LeaveService>();
            


            #endregion

            #region RepoServiceRegister
            //services.AddScoped<ICurrentSession, CurrentSession>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            #endregion


            #region RegisterConfig
            //services.Configure<LoggingSettings>(
            //configuration.GetSection("LoggingSettings"));

            services.Configure<EmailSettings>(
                configuration.GetSection("EmailSettings"));

            //services.Configure<RedisSettings>(
            //configuration.GetSection("JWTConnectionStrings"));
            #endregion
        }
    }
}
