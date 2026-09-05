using MediStock360.Application.Interfaces;
using MediStock360.Domain.Interfaces;
using MediStock360.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data.Common;
using MediStock360.Infrastructure.Persistence;

namespace MediStock360.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ICurrentUserService _currentService;
        private readonly MedicalDbContext _context;
        private IDbContextTransaction? _transaction;
        public IGenericRepository<Client> ClientRepository { get; }
        public IGenericRepository<User> UserRepository { get; }
        public IGenericRepository<UserRole> UserRoleRepository { get; }
        public IGenericRepository<Role> RoleRepository { get; }
        public IGenericRepository<Permission> PermissionRepository { get; }
        public IGenericRepository<Permission> PerimssionRepository => PermissionRepository;
        public IGenericRepository<BusinessType> BusinessTypeRepository { get; }
        public IGenericRepository<City> CityRepository { get; }
        public IGenericRepository<ClientSubscription> ClientSubscriptionRepository { get; }
        public IGenericRepository<Country> CountryRepository { get; }
        public IGenericRepository<IsSyncDatum> IsSyncDataRepository { get; }
        public IGenericRepository<Menu> MenuRepository { get; }
        public IGenericRepository<State> StateRepository { get; }
        public IGenericRepository<Store> StoreRepository { get; }
        public IGenericRepository<StoreUserMap> StoreUserMapRepository { get; }
        public IGenericRepository<SubscriptionPlan> SubscriptionPlanRepository { get; }
        public IGenericRepository<UserOtp> UserOtpRepository { get; }
        public IGenericRepository<NotificationTemplate> NotificationTemplateRepository { get; }
        public IGenericRepository<AppSetting> AppSettingRepository { get; }
        public IGenericRepository<DatabaseVersion> DatabaseVersionRepository { get; }
        public IGenericRepository<MasterCodeGeneration> MasterCodeGenerationRepository { get; }
        public IGenericRepository<RolePermission> RolePermissionRepository { get; }
        public UnitOfWork(MedicalDbContext context,ICurrentUserService currentUser)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _currentService = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
            ClientRepository = new GenericRepository<Client>(_context, _currentService);
            UserRepository = new GenericRepository<User>(_context, _currentService);
            UserRoleRepository = new GenericRepository<UserRole>(_context, _currentService);
            RoleRepository = new GenericRepository<Role>(_context, _currentService);
            PermissionRepository = new GenericRepository<Permission>(_context, _currentService);
            BusinessTypeRepository = new GenericRepository<BusinessType>(_context, _currentService);
            CityRepository = new GenericRepository<City>(_context, _currentService);
            ClientSubscriptionRepository = new GenericRepository<ClientSubscription>(_context, _currentService);
            CountryRepository = new GenericRepository<Country>(_context, _currentService);
            IsSyncDataRepository = new GenericRepository<IsSyncDatum>(_context, _currentService);
            MenuRepository = new GenericRepository<Menu>(_context, _currentService);
            StateRepository = new GenericRepository<State>(_context, _currentService);
            StoreRepository = new GenericRepository<Store>(_context, _currentService);
            StoreUserMapRepository = new GenericRepository<StoreUserMap>(_context, _currentService);
            SubscriptionPlanRepository = new GenericRepository<SubscriptionPlan>(_context, _currentService);
            UserOtpRepository = new GenericRepository<UserOtp>(_context, _currentService);
            NotificationTemplateRepository = new GenericRepository<NotificationTemplate>(_context, _currentService);
            RolePermissionRepository = new GenericRepository<RolePermission>(_context, _currentService);
            AppSettingRepository = new GenericRepository<AppSetting>(_context, _currentService);
            DatabaseVersionRepository = new GenericRepository<DatabaseVersion>(_context, _currentService);
            MasterCodeGenerationRepository = new GenericRepository<MasterCodeGeneration>(_context, _currentService);
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







