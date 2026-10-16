
using HRMS.Application.Services;
using MediStock360.Application.Common;
using MediStock360.Application.Interface;
using MediStock360.Application.Interfaces;
using MediStock360.Application.Services;
using MediStock360.Domain.Interfaces;
using MediStock360.Infrastructure.Interfaces;
using MediStock360.Infrastructure.Persistence;
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
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IRedisCacheService, RedisCacheService>();
            services.AddScoped<ISettingService, SettingService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IOTPService, OTPService>();
            services.AddScoped<IPermissionService, PermissionService>();
            services.AddScoped<IMenuService, MenuService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IMasterService, MasterService>();
            services.AddScoped<IClientService, ClientService>();
            services.AddScoped<IStoreService, StoreService>();
            services.AddScoped<ISubscriptionPlanService, SubscriptionPlanService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IProductOwnerService, ProductOwnerService>();
            services.AddScoped<IDatabaseSeeder, DatabaseSeeder>();
            services.AddHttpClient();
            //services.AddScoped<ITelegramBotService, TelegramBotService>();
            services.AddHttpClient<ITelegramBotService, TelegramBotService>();
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

            services.Configure<TelegramSettings>(
                configuration.GetSection("TelegramSettings"));

            services.Configure<ProductOwnerUserSeedSettings>(
                configuration.GetSection("ProductOwnerUser"));

            services.Configure<InternalClientSeedSettings>(
                configuration.GetSection("InternalClient"));

            //services.Configure<RedisSettings>(
            //configuration.GetSection("JWTConnectionStrings"));
            #endregion
        }
    }
}
