using Microsoft.EntityFrameworkCore;

namespace Berger.Extensions.EntityFrameworkCore
{
    public static class SeedExtensions
    {
        public static Task AddSeedAsync<TEntity>(this DbContext context, Func<IEnumerable<TEntity>> factory, CancellationToken token = default) where TEntity : class
        {
            return context.Set<TEntity>().AddRangeAsync(factory(), token);
        }

        public static async Task AddSeedsAsync(this DbContext context, IEnumerable<Func<DbContext, CancellationToken, Task>> seeds, CancellationToken token = default)
        {
            foreach (Func<DbContext, CancellationToken, Task> seed in seeds)
            {
                await seed(context, token);
            }
        }
    }
}