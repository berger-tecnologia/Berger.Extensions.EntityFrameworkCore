
using Microsoft.Extensions.DependencyInjection;

namespace Berger.Extensions.EntityFrameworkCore;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEntityFrameworkCoreExtensions<TContext>(this IServiceCollection services, Action<EntityFrameworkCoreOptions>? configure = null) where TContext : DbContext
    {
        var options = new EntityFrameworkCoreOptions();
        configure?.Invoke(options);

        services.AddOptions<EntityFrameworkCoreOptions>().Configure(current =>
        {
            current.QueryTrackingBehavior = options.QueryTrackingBehavior;
            current.RegisterRepository = options.RegisterRepository;
            current.UseSoftDeleteQueryFilter = options.UseSoftDeleteQueryFilter;
            current.UseNoActionDeleteBehavior = options.UseNoActionDeleteBehavior;
            current.CommandTimeoutSeconds = options.CommandTimeoutSeconds;
        });

        return options.RegisterRepository ? services.AddScoped(typeof(IRepository<>), typeof(Repository<>)).AddScoped<IUnitOfWork, UnitOfWork<TContext>>() : services;
    }
}