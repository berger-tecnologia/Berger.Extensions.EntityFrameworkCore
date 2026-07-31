namespace Berger.Extensions.EntityFrameworkCore;

public static class SpecificationEvaluator
{
    public static IQueryable<T> Apply<T>(IQueryable<T> query, ISpecification<T> specification) where T : class
    {
        ArgumentNullException.ThrowIfNull(query);
        ArgumentNullException.ThrowIfNull(specification);

        query = specification.AsNoTracking ? query.AsNoTracking() : query;
        query = specification.IgnoreQueryFilters ? query.IgnoreQueryFilters() : query;
        query = specification.Criteria is null ? query : query.Where(specification.Criteria);
        query = specification.Includes.Aggregate(query, static (current, include) => current.Include(include));
        query = specification.OrderBy?.Invoke(query) ?? query;
        query = specification.Skip is > 0 ? query.Skip(specification.Skip.Value) : query;
        query = specification.Take is > 0 ? query.Take(specification.Take.Value) : query;

        return query;
    }
}
