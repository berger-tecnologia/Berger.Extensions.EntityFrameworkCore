using System.Text;
using System.Text.RegularExpressions;


namespace Berger.Extensions.EntityFrameworkCore;

public static partial class SqlScript
{
    public static IReadOnlyList<string> SplitBatches(string script) => string.IsNullOrWhiteSpace(script) ? [] : GoCommandRegex().Split(Normalize(script)).Select(static batch => batch.Trim()).Where(static batch => batch.Length > 0).ToArray();

    public static async Task ExecuteFileAsync(this DbContext context, string path, CancellationToken cancellationToken = default)
    {
        var script = await File.ReadAllTextAsync(ValidatePath(path), Encoding.UTF8, cancellationToken);
        await context.ExecuteScriptAsync(script, cancellationToken);
    }

    public static async Task ExecuteScriptAsync(this DbContext context, string script, CancellationToken cancellationToken = default)
    {
        foreach (var batch in SplitBatches(script))
        {
            await context.Database.ExecuteSqlRawAsync(batch, cancellationToken);
        }
    }

    private static string Normalize(string script) => script.Replace("\r\n", "\n", StringComparison.Ordinal).Replace('\r', '\n');
    private static string ValidatePath(string path) => File.Exists(path) ? path : throw new FileNotFoundException("SQL script file was not found.", path);

    [GeneratedRegex(@"^\s*GO(?:\s+\d+)?\s*(?:--.*)?$", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.CultureInvariant)]
    private static partial Regex GoCommandRegex();
}
