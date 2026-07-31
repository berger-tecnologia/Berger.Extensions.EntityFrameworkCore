using Microsoft.EntityFrameworkCore.Metadata;

namespace Berger.Extensions.EntityFrameworkCore;

public abstract class BaseEntityTypeConfiguration<T> : IEntityTypeConfiguration<T> where T : BaseEntity
{
    public virtual void Configure(EntityTypeBuilder<T> builder)
    {

        //builder.HasQueryFilter(QueryFilterNames.SoftDelete, entity => !entity.Deleted);

        builder.Property(e => e.Deleted).HasColumnName(BaseColumns.IsDeleted);
        builder.Property(e => e.CreatedOn).HasColumnName(BaseColumns.CreatedOn).ValueGeneratedOnAdd().Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
        builder.Property(e => e.UpdatedOn).HasColumnName(BaseColumns.UpdatedOn);
        builder.Property(e => e.DeletedOn).HasColumnName(BaseColumns.DeletedOn);
        builder.HasIndex(e => e.CreatedOn);

        ConfigureEntity(builder);
    }

    protected virtual void ConfigureEntity(EntityTypeBuilder<T> builder)
    {
    }
}