using System.Linq.Expressions;

namespace Berger.Extensions.EntityFrameworkCore;

public class Repository<T>(DbContext context) : IRepository<T> where T : class
{
    private readonly DbContext _context =
        context ?? throw new ArgumentNullException(nameof(context));

    public IQueryable<T> Select(bool tracking = false)
    {
        return Query(tracking);
    }
    public IQueryable<T> Select(ISpecification<T> specification)
    {
        return SpecificationEvaluator.Apply(Set(), specification);
    }
    public ValueTask<T?> GetByIdAsync<TId>(TId id, CancellationToken token = default) where TId : notnull
    {
        return Set().FindAsync([id], token);
    }
    public Task<T?> GetOneAsync(Expression<Func<T, bool>> predicate, CancellationToken token = default)
    {
        return Query()
            .FirstOrDefaultAsync(predicate, token);
    }
    public async Task<IReadOnlyList<T>> GetAsync(CancellationToken token = default)
    {
        return await Query()
            .ToListAsync(token);
    }
    public async Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate, CancellationToken token = default)
    {
        return await Query()
            .Where(predicate)
            .ToListAsync(token);
    }
    public async Task<IReadOnlyList<T>> GetAsync(ISpecification<T> specification, CancellationToken token = default)
    {
        return await Select(specification)
            .ToListAsync(token);
    }
    public Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken token = default)
    {
        return Set()
            .AnyAsync(predicate, token);
    }
    public Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken token = default)
    {
        return Query()
            .Apply(predicate)
            .CountAsync(token);
    }
    public async Task<T> InsertAsync(T entity, bool saveChanges = true, CancellationToken token = default)
    {
        return await SaveAsync(
            entity,
            saveChanges,
            token,
            static (set, value, token) =>
                set.AddAsync(value, token).AsTask());
    }
    public async Task InsertAsync(IEnumerable<T> entities, bool saveChanges = true, CancellationToken token = default)
    {
        await SaveAsync(
            entities,
            saveChanges,
            token,
            static (set, values, token) =>
                set.AddRangeAsync(values, token));
    }
    public async Task<T> UpdateAsync(T entity, bool saveChanges = true, CancellationToken token = default)
    {
        return await SaveAsync(
            entity,
            saveChanges,
            token,
            static (set, value, _) =>
            {
                set.Update(value);

                return Task.CompletedTask;
            });
    }
    public async Task DeleteAsync(T entity, bool saveChanges = true, CancellationToken token = default)
    {
        await SaveAsync(
            entity,
            saveChanges,
            token,
            static (set, value, _) =>
            {
                set.Remove(value);

                return Task.CompletedTask;
            });
    }
    public async Task DeleteAsync(IEnumerable<T> entities, bool saveChanges = true, CancellationToken token = default)
    {
        await SaveAsync(
            entities,
            saveChanges,
            token,
            static (set, values, _) =>
            {
                set.RemoveRange(values);

                return Task.CompletedTask;
            });
    }
    public Task<int> DeleteWhereAsync(Expression<Func<T, bool>> predicate, CancellationToken token = default)
    {
        return Set()
            .Where(predicate)
            .ExecuteDeleteAsync(token);
    }
    public Task<int> SaveChangesAsync(CancellationToken token = default)
    {
        return _context.SaveChangesAsync(token);
    }
    public async Task<PagedResult<T>> PageAsync(int page, int pageSize, Expression<Func<T, bool>>? predicate = null, CancellationToken token = default)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(page, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(pageSize, 1);

        var query = Query()
            .Apply(predicate);

        var total = await query
            .LongCountAsync(token);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(token);

        return new PagedResult<T>(
            items,
            page,
            pageSize,
            total);
    }
    public async Task SoftDeleteAsync(T entity, bool saveChanges = true, CancellationToken token = default)
    {
        var deletedEntity =
            entity as IDeleted
            ?? throw new InvalidOperationException(
                $"Entity '{typeof(T).Name}' must implement '{nameof(IDeleted)}' to support soft delete.");

        deletedEntity.Delete();

        Set().Update(entity);

        await CommitAsync(saveChanges, token);
    }
    private DbSet<T> Set()
    {
        return _context.Set<T>();
    }
    private IQueryable<T> Query(bool tracking = false)
    {
        return tracking
            ? Set()
            : Set().AsNoTracking();
    }
    private Task CommitAsync(bool saveChanges, CancellationToken token)
    {
        return saveChanges
            ? SaveChangesAsync(token)
            : Task.CompletedTask;
    }
    private async Task<TValue> SaveAsync<TValue>(TValue value, bool saveChanges, CancellationToken token, Func<DbSet<T>, TValue, CancellationToken, Task> operation)
    {
        await operation(Set(), value, token);
        await CommitAsync(saveChanges, token);

        return value;
    }
}

file static class RepositoryQueryExtensions
{
    public static IQueryable<T> Apply<T>(this IQueryable<T> query, Expression<Func<T, bool>>? predicate) where T : class
    {
        return predicate is null
            ? query
            : query.Where(predicate);
    }
}