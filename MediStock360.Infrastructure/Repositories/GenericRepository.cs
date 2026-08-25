using MediStock360.Application.Interfaces;
using MediStock360.Infrastructure.Interfaces;
using MediStock360.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace MediStock360.Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly MedicalDbContext _context;
        protected readonly DbSet<T> _dbSet;
        protected readonly ICurrentUserService _currentUser;

        public GenericRepository(MedicalDbContext context, ICurrentUserService currentUser)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbSet = _context.Set<T>();   // ✅ ALWAYS initialize here
            _currentUser = currentUser;
        }

        //public GenericRepository(HRMSDbRepoContext context)
        //{
        //    _context = context;
        //    _dbSet = context.Set<T>();
        //}

        // -------------------- READ --------------------

        public async Task<List<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }
        public async Task<T?> GetByIdAsync(object id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.FirstOrDefaultAsync(predicate);
        }

        public async Task<List<T>> WhereAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public async Task<TResult?> MaxAsync<TResult>(Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> selector) { return await _dbSet.Where(predicate).Select(selector).DefaultIfEmpty().MaxAsync(); }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }

        // -------------------- WRITE --------------------

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            //foreach (var entity in entities)
            //{
            //    entity.CreatedAt = DateTime.UtcNow;
            //    entity.CreatedBy = _currentUser.UserId;
            //}

            await _dbSet.AddRangeAsync(entities);
        }

        public void Update(T entity)
        {
            //entity.UpdatedAt = DateTime.UtcNow;
            //entity.UpdatedBy = _currentUser.UserId;
            _dbSet.Update(entity);
        }
        public async Task SoftDeleteAsync(T entity)
        {
            _dbSet.Update(entity);

            await Task.CompletedTask;
        }

        public async Task ActivateAsync(T entity)
        {
            _dbSet.Update(entity);

            await Task.CompletedTask;
        }

        public void Remove(T entity)
        {
            _dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
        }

        // -------------------- ADVANCED --------------------

        public IQueryable<T> Query()
        {
            return _dbSet.AsQueryable();
        }
    }
}
