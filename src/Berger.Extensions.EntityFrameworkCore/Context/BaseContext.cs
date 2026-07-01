using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Berger.Extensions.EntityFrameworkCore;

public abstract class BaseContext<TContext>(DbContextOptions<TContext> options, TimeProvider? timeProvider = null) : DbContext(options) where TContext : DbContext
{
    private readonly TimeProvider _timeProvider = timeProvider ?? TimeProvider.System;

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<string>().AreUnicode(false).HaveMaxLength(4000);
        configurationBuilder.Properties<decimal>().HavePrecision(28, 6);
        configurationBuilder.Properties<DateTime>().HavePrecision(3);
        configurationBuilder.Properties<DateTimeOffset>().HavePrecision(3);
        configurationBuilder.Properties<List<string>>().HaveConversion<StringListConverter>().AreUnicode(false).HaveMaxLength(4000);

        base.ConfigureConventions(configurationBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ConfigureConventions();

        base.OnModelCreating(modelBuilder);
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        ApplyAuditMetadata();

        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken token = default)
    {
        ApplyAuditMetadata();

        return base.SaveChangesAsync(acceptAllChangesOnSuccess, token);
    }

    private void ApplyAuditMetadata()
    {
        var now = _timeProvider.GetUtcNow().UtcDateTime;

        foreach (var entry in ChangeTracker.Entries<IAuditable>())
        {
            ApplyAuditMetadata(entry, now);
        }
    }
    private static void ApplyAuditMetadata(EntityEntry<IAuditable> entry, DateTime now)
    {
        _ = entry.State switch
        {
            EntityState.Added => ApplyCreatedMetadata(entry, now),
            EntityState.Modified => ApplyUpdatedMetadata(entry, now),
            EntityState.Deleted => ApplyDeletedMetadata(entry, now),
            _ => 0
        };
    }
    private static int ApplyCreatedMetadata(EntityEntry<IAuditable> entry, DateTime now)
    {
        entry.Entity.CreatedOn = entry.Entity.CreatedOn == default ? now : entry.Entity.CreatedOn;
        entry.Entity.Deleted = false;

        return 0;
    }

    private static int ApplyUpdatedMetadata(EntityEntry<IAuditable> entry, DateTime now)
    {
        entry.Entity.UpdatedOn = now;
        entry.Property(nameof(IAuditable.CreatedOn)).IsModified = false;
        return 0;
    }

    private static int ApplyDeletedMetadata(EntityEntry<IAuditable> entry, DateTime now)
    {
        entry.State = EntityState.Modified;
        entry.Entity.Deleted = true;
        entry.Entity.DeletedOn = now;

        entry.Property(nameof(IAuditable.CreatedOn)).IsModified = false;
        return 0;
    }
}