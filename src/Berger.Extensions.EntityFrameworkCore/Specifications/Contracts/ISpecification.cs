using System.Linq.Expressions;

namespace Berger.Extensions.EntityFrameworkCore;

public interface ISpecification<T> where T : class
{
    Expression<Func<T, bool>>? Criteria { get; }
    IReadOnlyList<Expression<Func<T, object?>>> Includes { get; }
    Func<IQueryable<T>, IOrderedQueryable<T>>? OrderBy { get; }
    int? Skip { get; }
    int? Take { get; }
    bool AsNoTracking { get; }
    bool IgnoreQueryFilters { get; }
}
