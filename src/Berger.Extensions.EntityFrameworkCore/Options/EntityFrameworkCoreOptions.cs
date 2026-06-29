
namespace Berger.Extensions.EntityFrameworkCore;

public sealed class EntityFrameworkCoreOptions
{
    public QueryTrackingBehavior QueryTrackingBehavior { get; set; } = QueryTrackingBehavior.NoTracking;
    public bool RegisterRepository { get; set; } = true;
    public bool UseSoftDeleteQueryFilter { get; set; } = true;
    public bool UseNoActionDeleteBehavior { get; set; } = true;
    public int? CommandTimeoutSeconds { get; set; }
}