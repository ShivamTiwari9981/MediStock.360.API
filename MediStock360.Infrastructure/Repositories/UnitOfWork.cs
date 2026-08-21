
using MediStock360.Domain.Entities;
using MediStock360.Infrastructure.Interfaces;
using MediStock360.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data.Common;

namespace MediStock360.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        //private readonly ICurrentUserService _currentService;
        private readonly MedicalDbContext _context;
        private IDbContextTransaction? _transaction;
        public IGenericRepository<Client> ClientRepository { get; }
        public IGenericRepository<User> UserRepository { get; }
        public IGenericRepository<UserRole> UserRoleRepository { get; }
        public IGenericRepository<Role> RoleRepository { get; }
        public IGenericRepository<Permission> PerimssionRepository { get; }
        public IGenericRepository<BusinessType> BusinessTypeRepository { get; }
        public IGenericRepository<City> CityRepository { get; }
        public IGenericRepository<ClientSubscription> ClientSubscriptionRepository { get; }
        public IGenericRepository<Country> CountryRepository { get; }
        public IGenericRepository<IsSyncDatum> IsSyncDataRepository { get; }
        public IGenericRepository<Menu> MenuRepository { get; }
        public IGenericRepository<State> StateRepository { get; }
        public IGenericRepository<Store> StoreRepository { get; }
        public IGenericRepository<SubscriptionPlan> SubscriptionPlanRepository { get; }
        public UnitOfWork(MedicalDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            //_currentService = currentUser ?? throw new ArgumentNullException(nameof(currentUser));

        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            if (_transaction != null)
                return _transaction;

            _transaction = await _context.Database.BeginTransactionAsync();

            return _transaction;
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                await _context.SaveChangesAsync();

                if (_transaction != null)
                {
                    await _transaction.CommitAsync();
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
            catch
            {
                await RollbackTransactionAsync();
                throw;
            }
        }


        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public DbConnection GetConnection()
        {
            return _context.Database.GetDbConnection();
        }

        // ✅ Get Current Transaction
        public DbTransaction? GetTransaction()
        {
            return _transaction?.GetDbTransaction();
        }

        // ✅ Get Connection String
        public string GetConnectionString()
        {
            return _context.Database.GetConnectionString();
        }


        
    }
}




