using Microsoft.EntityFrameworkCore;
using Berger.Extensions.Abstractions;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Berger.Extensions.EntityFrameworkCore
{
    public abstract class BaseConfigurationMap<T> : IEntityTypeConfiguration<T> where T : class, IBaseEntity<Guid>
    {
        public virtual void Configure(EntityTypeBuilder<T> builder)
        {
            var properties = typeof(T).GetProperties();

            /* Primary key */
            builder.HasKey(e => e.ID);

            // Field names
            builder.Property(e => e.Deleted).HasColumnName(BaseColumns.IsDeleted);
            builder.Property(e => e.DeletedOn).HasColumnName(BaseColumns.DeletedOn);
            builder.Property(e => e.CreatedOn).HasColumnName(BaseColumns.CreatedOn);
            builder.Property(e => e.ModifiedOn).HasColumnName(BaseColumns.ModifiedOn);

            // Mandatory fields
            builder.Property(e => e.Deleted).IsRequired();
            builder.Property(e => e.CreatedOn).IsRequired();

            /* Default values */
            builder.Property(e => e.Deleted).HasDefaultValue(false);
            builder.Property(e => e.CreatedOn).HasDefaultValue(DateTime.UtcNow);

            /* Query Filters */
            builder.HasQueryFilter(e => !e.Deleted);

            /* Field lock */
            builder.Property(e => e.CreatedOn).ValueGeneratedOnAdd().Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

            /* Indexes */
            builder.HasIndex(e => e.CreatedOn);

            /* Column Order */
            builder.Property(KeyColumns.ID).HasColumnOrder(0);

            // Base configuration
            builder.Configure<T>();
        }
    }
}