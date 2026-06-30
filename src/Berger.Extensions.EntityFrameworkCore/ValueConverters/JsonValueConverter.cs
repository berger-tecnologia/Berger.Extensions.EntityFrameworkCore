using System.Text.Json;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Berger.Extensions.EntityFrameworkCore;

public class JsonValueConverter<T>(JsonSerializerOptions? options = null) : ValueConverter<T?, string?>(value => Serialize(value, options), value => Deserialize(value, options))
{
    private static string? Serialize(T? value, JsonSerializerOptions? options) => value is null ? null : JsonSerializer.Serialize(value, options);
    private static T? Deserialize(string? value, JsonSerializerOptions? options) => string.IsNullOrWhiteSpace(value) ? default : JsonSerializer.Deserialize<T>(value, options);
}
