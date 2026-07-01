namespace Berger.Extensions.EntityFrameworkCore
{
    public interface IRepositoryModule
    {
        int Order { get; }
        void Configure(ModelBuilder modelBuilder);
        Task SeedAsync(DbContext context, CancellationToken token = default);
    }
}