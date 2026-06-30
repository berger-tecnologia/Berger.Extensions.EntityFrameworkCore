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
    public ValueTask<T?> GetByIdAsync<TId>(TId id, CancellationToken cancellationToken = default) where TId : notnull
    {
        return Set().FindAsync([id], cancellationToken);
    }
    public Task<T?> GetOneAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return Query()
            .FirstOrDefaultAsync(predicate, cancellationToken);
    }
    public async Task<IReadOnlyList<T>> GetAsync(CancellationToken cancellationToken = default)
    {
        return await Query()
            .ToListAsync(cancellationToken);
    }
    public async Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await Query()
            .Where(predicate)
            .ToListAsync(cancellationToken);
    }
    public async Task<IReadOnlyList<T>> GetAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
    {
        return await Select(specification)
            .ToListAsync(cancellationToken);
    }
    public Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return Set()
            .AnyAsync(predicate, cancellationToken);
    }
    public Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        return Query()
            .Apply(predicate)
            .CountAsync(cancellationToken);
    }
    public async Task<T> InsertAsync(T entity, bool saveChanges = true, CancellationToken cancellationToken = default)
    {
        return await SaveAsync(
            entity,
            saveChanges,
            cancellationToken,
            static (set, value, token) =>
                set.AddAsync(value, token).AsTask());
    }
    public async Task InsertAsync(IEnumerable<T> entities, bool saveChanges = true, CancellationToken cancellationToken = default)
    {
        await SaveAsync(
            entities,
            saveChanges,
            cancellationToken,
            static (set, values, token) =>
                set.AddRangeAsync(values, token));
    }
    public async Task<T> UpdateAsync(T entity, bool saveChanges = true, CancellationToken cancellationToken = default)
    {
        return await SaveAsync(
            entity,
            saveChanges,
            cancellationToken,
            static (set, value, _) =>
            {
                set.Update(value);

                return Task.CompletedTask;
            });
    }
    public async Task DeleteAsync(T entity, bool saveChanges = true, CancellationToken cancellationToken = default)
    {
        await SaveAsync(
            entity,
            saveChanges,
            cancellationToken,
            static (set, value, _) =>
            {
                set.Remove(value);

                return Task.CompletedTask;
            });
    }
    public async Task DeleteAsync(IEnumerable<T> entities, bool saveChanges = true, CancellationToken cancellationToken = default)
    {
        await SaveAsync(
            entities,
            saveChanges,
            cancellationToken,
            static (set, values, _) =>
            {
                set.RemoveRange(values);

                return Task.CompletedTask;
            });
    }
    public Task<int> DeleteWhereAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return Set()
            .Where(predicate)
            .ExecuteDeleteAsync(cancellationToken);
    }
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
    public async Task<PagedResult<T>> PageAsync(int page, int pageSize, Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(page, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(pageSize, 1);

        var query = Query()
            .Apply(predicate);

        var total = await query
            .LongCountAsync(cancellationToken);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<T>(
            items,
            page,
            pageSize,
            total);
    }
    public async Task SoftDeleteAsync(T entity, bool saveChanges = true, CancellationToken cancellationToken = default)
    {
        var deletedEntity =
            entity as IDeleted
            ?? throw new InvalidOperationException(
                $"Entity '{typeof(T).Name}' must implement '{nameof(IDeleted)}' to support soft delete.");

        deletedEntity.Delete();

        Set().Update(entity);

        await CommitAsync(saveChanges, cancellationToken);
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
    private Task CommitAsync(bool saveChanges, CancellationToken cancellationToken)
    {
        return saveChanges
            ? SaveChangesAsync(cancellationToken)
            : Task.CompletedTask;
    }
    private async Task<TValue> SaveAsync<TValue>(TValue value, bool saveChanges, CancellationToken cancellationToken, Func<DbSet<T>, TValue, CancellationToken, Task> operation)
    {
        await operation(Set(), value, cancellationToken);
        await CommitAsync(saveChanges, cancellationToken);

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