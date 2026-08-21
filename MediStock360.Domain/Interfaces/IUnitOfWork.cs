
using MediStock360.Domain.Entities;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data.Common;

namespace MediStock360.Infrastructure.Interfaces
{
    public interface IUnitOfWork
    {
        IGenericRepository<Client> ClientRepository { get; }
        IGenericRepository<User> UserRepository { get; }
        IGenericRepository<UserRole> UserRoleRepository { get; }
        IGenericRepository<Role> RoleRepository { get; }
        IGenericRepository<Permission> PerimssionRepository { get; }
        IGenericRepository<BusinessType> BusinessTypeRepository { get; }
        IGenericRepository<City> CityRepository { get; }
        IGenericRepository<ClientSubscription> ClientSubscriptionRepository { get; }
        IGenericRepository<Country> CountryRepository { get; }
        IGenericRepository<IsSyncDatum> IsSyncDataRepository { get; }
        IGenericRepository<Menu> MenuRepository { get; }
        IGenericRepository<State> StateRepository { get; }
        IGenericRepository<Store> StoreRepository { get; }
        IGenericRepository<SubscriptionPlan> SubscriptionPlanRepository { get; }
        Task<bool> SaveChangesAsync();
        Task<IDbContextTransaction>BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();

        //void Dispose();

        DbConnection GetConnection();

        DbTransaction GetTransaction();

        string GetConnectionString();
    }
}
