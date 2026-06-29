using System.Linq.Expressions;

namespace Berger.Extensions.EntityFrameworkCore;

public interface IRepository<T> where T : class
{
    IQueryable<T> Select(bool tracking = false);
    IQueryable<T> Select(ISpecification<T> specification);
    ValueTask<T?> GetByIdAsync<TId>(TId id, CancellationToken cancellationToken = default) where TId : notnull;
    Task<T?> GetOneAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<T>> GetAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<T>> GetAsync(ISpecification<T> specification, CancellationToken cancellationToken = default);
    Task<PagedResult<T>> PageAsync(int page, int pageSize, Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default);
    Task<T> InsertAsync(T entity, bool saveChanges = true, CancellationToken cancellationToken = default);
    Task InsertAsync(IEnumerable<T> entities, bool saveChanges = true, CancellationToken cancellationToken = default);
    Task<T> UpdateAsync(T entity, bool saveChanges = true, CancellationToken cancellationToken = default);
    Task DeleteAsync(T entity, bool saveChanges = true, CancellationToken cancellationToken = default);
    Task DeleteAsync(IEnumerable<T> entities, bool saveChanges = true, CancellationToken cancellationToken = default);
    Task SoftDeleteAsync(T entity, bool saveChanges = true, CancellationToken cancellationToken = default);
    Task<int> DeleteWhereAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
