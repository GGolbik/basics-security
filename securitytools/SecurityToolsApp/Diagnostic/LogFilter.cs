using Serilog.Events;

namespace GGolbik.SecurityTools.Diagnostic;

public class LogFilter
{
    public ulong Limit { get; set; } = 0;
    public ulong Offset { get; set; } = 0;
    public SortDirection Direction { get; set; } = SortDirection.Descending;
    public IList<string>? FilterProperties { get; set; } = null;
    public IList<string>? FilterRenderedMessage { get; set; } = null;
    public IList<string>? FilterExceptions { get; set; } = null;
    public IList<LogEventLevel>? FilterLevel { get; set; } = null;
    public DateTime? Since { get; set; } = null;
    public DateTime? Until { get; set; } = null;
}