using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Berger.Extensions.EntityFrameworkCore;

public class StringListConverter() : ValueConverter<List<string>, string>(value => string.Join(',', value), value => value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList());
