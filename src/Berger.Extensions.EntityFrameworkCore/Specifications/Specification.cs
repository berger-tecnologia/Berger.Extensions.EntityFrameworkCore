using System.Linq.Expressions;

namespace Berger.Extensions.EntityFrameworkCore;

public abstract class Specification<T> : ISpecification<T> where T : class
{
    private readonly List<Expression<Func<T, object?>>> _includes = [];

    public Expression<Func<T, bool>>? Criteria { get; private set; }
    public IReadOnlyList<Expression<Func<T, object?>>> Includes => _includes;
    public Func<IQueryable<T>, IOrderedQueryable<T>>? OrderBy { get; private set; }
    public int? Skip { get; private set; }
    public int? Take { get; private set; }
    public bool AsNoTracking { get; private set; } = true;
    public bool IgnoreQueryFilters { get; private set; }

    protected void Where(Expression<Func<T, bool>> criteria) => Criteria = criteria ?? throw new ArgumentNullException(nameof(criteria));
    protected void Include(Expression<Func<T, object?>> include) => _includes.Add(include ?? throw new ArgumentNullException(nameof(include)));
    protected void OrderByAscending<TKey>(Expression<Func<T, TKey>> property) => OrderBy = query => query.OrderBy(property ?? throw new ArgumentNullException(nameof(property)));
    protected void OrderByDescending<TKey>(Expression<Func<T, TKey>> property) => OrderBy = query => query.OrderByDescending(property ?? throw new ArgumentNullException(nameof(property)));
    protected void WithTracking() => AsNoTracking = false;
    protected void WithoutQueryFilters() => IgnoreQueryFilters = true;

    protected void Page(int page, int pageSize)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(page, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(pageSize, 1);
        Skip = (page - 1) * pageSize;
        Take = pageSize;
    }
}
