
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Berger.Extensions.EntityFrameworkCore;

public static class DbContextOptionsBuilderExtensions
{
    public static DbContextOptionsBuilder UseSqlServerConnection(this DbContextOptionsBuilder builder, IConfiguration configuration, string connectionStringName = ConnectionStringNames.Default, Action<SqlServerDbContextOptionsBuilder>? sqlServer = null)
    {
        var connectionString = configuration.GetConnectionString(connectionStringName);

        return builder.UseSqlServer(string.IsNullOrWhiteSpace(connectionString) ? throw new InvalidOperationException($"Connection string '{connectionStringName}' was not found.") : connectionString, sqlServer);
    }
}