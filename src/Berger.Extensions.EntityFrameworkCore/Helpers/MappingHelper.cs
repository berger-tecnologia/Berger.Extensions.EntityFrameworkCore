using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Berger.Extensions.EntityFrameworkCore
{
    public static class MappingHelper
    {
        public static void Configure<T>(this EntityTypeBuilder builder)
        {
            var rules = CustomRelational.GetRules();

            var properties = typeof(T).GetProperties();
            var interfaces = typeof(T).GetInterfaces();

            foreach (var property in properties)
            {
                FieldRelational rule;

                if (rules.TryGetValue(property.Name, out rule))
                {
                    if (!string.IsNullOrEmpty(rule.Name))
                        builder.Property(property.Name).HasColumnName(rule.Name);
                }

                // Ownership Rules
                if (property.Name == UserColumns.OwnerID)
                    builder.Property(UserColumns.OwnerID).ValueGeneratedOnAdd().Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
            }

            // Customization for fields based on his interface.
            var dictionary = rules.Where(e => interfaces.Any(b => e.Value.Type == b));

            foreach (var item in dictionary)
            {
                var key = item.Key;
                var rule = item.Value;

                if (rule.Index)
                    builder.HasIndex(key);

                if (rule.Value is not null)
                    builder.Property(key).HasDefaultValue(rule.Value);
            }
        }
    }
}