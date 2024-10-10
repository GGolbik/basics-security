using GGolbik.SecurityTools.X509.Models;

namespace GGolbik.SecurityTools.Diagnostic;

public class LogEntry
{
    /// <summary>
    /// Indicates a particular version of the <see cref="LogEntry"/>.
    /// </summary>
    public SchemaVersion? SchemaVersion { get; set; } = "1.0";

    public long? Id { get; set; }
    public DateTime? Timestamp { get; set; }
    public LogLevel? Level { get; set; }
    public string? Exception { get; set; }
    public string? Message { get; set; }
    public string? Properties { get; set; }
}
