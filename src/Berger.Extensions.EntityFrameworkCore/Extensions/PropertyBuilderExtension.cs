namespace Berger.Extensions.EntityFrameworkCore
{
    public static class PropertyBuilderExtensions
    {
        public static PropertyBuilder<TEnum> HasEnumConversion<TEnum>(this PropertyBuilder<TEnum> builder, string column) where TEnum : struct, Enum
        {
            return builder.HasConversion<string>().HasColumnName(column);
        }
    }
}