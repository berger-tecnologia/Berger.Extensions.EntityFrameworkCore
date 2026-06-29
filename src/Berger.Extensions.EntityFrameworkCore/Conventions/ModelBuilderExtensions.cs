using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Berger.Extensions.EntityFrameworkCore;

public static class ModelBuilderExtensions
{
    public static ModelBuilder ConfigureConventions(this ModelBuilder modelBuilder, bool softDelete = true, bool noActionDelete = true)
    {
        _ = softDelete ? modelBuilder.UseSoftDeleteQueryFilters() : modelBuilder;
        _ = noActionDelete ? modelBuilder.UseNoActionDeleteBehavior() : modelBuilder;
        
        return modelBuilder.UseBaseColumnNames();
    }

    public static ModelBuilder UseNoActionDeleteBehavior(this ModelBuilder modelBuilder)
    {
        foreach (var foreignKey in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
        {
            foreignKey.DeleteBehavior = DeleteBehavior.NoAction;
        }

        return modelBuilder;
    }

    public static ModelBuilder UseBaseColumnNames(this ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.EntityTypes<BaseEntity>())
        {
            var entity = modelBuilder.Entity(entityType.ClrType);

            entity.Property(nameof(BaseEntity.Deleted)).HasColumnName(BaseColumns.IsDeleted);
            entity.Property(nameof(BaseEntity.CreatedOn)).HasColumnName(BaseColumns.CreatedOn);
            entity.Property(nameof(BaseEntity.UpdatedOn)).HasColumnName(BaseColumns.UpdatedOn);
            entity.Property(nameof(BaseEntity.DeletedOn)).HasColumnName(BaseColumns.DeletedOn);
        }

        return modelBuilder;
    }

    public static ModelBuilder UseSoftDeleteQueryFilters(this ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.EntityTypes<BaseEntity>().Where(static e => e.BaseType is null))
        {
            modelBuilder.Entity(entityType.ClrType).HasQueryFilter(QueryFilterNames.SoftDelete, CreateSoftDeleteFilter(entityType.ClrType));
        }

        return modelBuilder;
    }

    private static IEnumerable<IMutableEntityType> EntityTypes<T>(this ModelBuilder modelBuilder) => modelBuilder.Model.GetEntityTypes().Where(entity => typeof(T).IsAssignableFrom(entity.ClrType));

    private static LambdaExpression CreateSoftDeleteFilter(Type entityType)
    {
        var parameter = Expression.Parameter(entityType, "entity");
        var deleted = Expression.Property(parameter, nameof(BaseEntity.Deleted));

        return Expression.Lambda(Expression.Equal(deleted, Expression.Constant(false)), parameter);
    }
}