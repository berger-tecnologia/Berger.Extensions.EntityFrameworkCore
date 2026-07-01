
using Microsoft.Extensions.DependencyInjection;

namespace Berger.Extensions.EntityFrameworkCore;

public class UnitOfWork<TContext>(TContext context, IServiceProvider serviceProvider) : IUnitOfWork where TContext : DbContext
{
    public IRepository<T> Repository<T>() where T : class => serviceProvider.GetRequiredService<IRepository<T>>();
    public Task<int> SaveChangesAsync(CancellationToken token = default) => context.SaveChangesAsync(token);
}
