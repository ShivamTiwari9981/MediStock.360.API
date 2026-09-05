using MediStock360.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data.Common;
using MediStock360.Infrastructure;

namespace MediStock360.Domain.Interfaces
{
    public interface IUnitOfWork
    {
        IGenericRepository<Client> ClientRepository { get; }
        IGenericRepository<AppSetting> AppSettingRepository { get; }
        IGenericRepository<User> UserRepository { get; }
        IGenericRepository<UserRole> UserRoleRepository { get; }
        IGenericRepository<Role> RoleRepository { get; }
        IGenericRepository<RolePermission> RolePermissionRepository { get; }
        IGenericRepository<Permission> PermissionRepository { get; }
        IGenericRepository<BusinessType> BusinessTypeRepository { get; }
        IGenericRepository<City> CityRepository { get; }
        IGenericRepository<ClientSubscription> ClientSubscriptionRepository { get; }
        IGenericRepository<Country> CountryRepository { get; }
        IGenericRepository<IsSyncDatum> IsSyncDataRepository { get; }
        IGenericRepository<Menu> MenuRepository { get; }
        IGenericRepository<State> StateRepository { get; }
        IGenericRepository<Store> StoreRepository { get; }
        IGenericRepository<StoreUserMap> StoreUserMapRepository { get; }
        IGenericRepository<SubscriptionPlan> SubscriptionPlanRepository { get; }
        IGenericRepository<UserOtp> UserOtpRepository { get; }
        IGenericRepository<NotificationTemplate> NotificationTemplateRepository { get; }
        Task<bool> SaveChangesAsync();
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();

        //void Dispose();

        DbConnection GetConnection();

        DbTransaction GetTransaction();

        string GetConnectionString();
    }
}
