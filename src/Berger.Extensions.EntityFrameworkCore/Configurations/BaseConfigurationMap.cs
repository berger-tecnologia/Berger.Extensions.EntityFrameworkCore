using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Berger.Extensions.EntityFrameworkCore
{
	public abstract class BaseConfigurationMap<T> : IEntityTypeConfiguration<T> where T : BaseEntity
	{
		public virtual void Configure(EntityTypeBuilder<T> builder)
		{
			var properties = typeof(T).GetProperties();

			if (builder.Metadata.BaseType == null)
			{
				builder.HasKey(e => e.Id);

				/* Query Filters */
				builder.HasQueryFilter(e => !e.Deleted);
			}

			// Field names
			builder.Property(e => e.Deleted).HasColumnName(BaseColumns.IsDeleted);
			builder.Property(e => e.DeletedOn).HasColumnName(BaseColumns.DeletedOn);
			builder.Property(e => e.CreatedOn).HasColumnName(BaseColumns.CreatedOn);

			// Mandatory fields
			builder.Property(e => e.Deleted).IsRequired();
			builder.Property(e => e.CreatedOn).IsRequired();

			/* Field lock */
			builder.Property(e => e.CreatedOn).ValueGeneratedOnAdd().Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

			/* Indexes */
			builder.HasIndex(e => e.CreatedOn);

			// Base configuration
			builder.Configure<T>();
		}
	}
}