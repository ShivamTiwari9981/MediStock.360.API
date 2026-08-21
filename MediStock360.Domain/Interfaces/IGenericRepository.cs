using System.Linq.Expressions;

namespace MediStock360.Infrastructure.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        // Read
        Task<List<T>> GetAllAsync();
        Task<T?> GetByIdAsync(object id);
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
        Task<List<T>> WhereAsync(Expression<Func<T, bool>> predicate);
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);

        Task<TResult?> MaxAsync<TResult>(Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> selector);

        // Write

        Task AddAsync(T entity);

        Task AddRangeAsync(IEnumerable<T> entities);

        void Update(T entity);

        Task SoftDeleteAsync(T entity);

        Task ActivateAsync(T entity);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);

        // Query (optional advanced)
        IQueryable<T> Query(); // ⚠️ use carefully
    }
}
