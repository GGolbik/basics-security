using GGolbik.SecurityTools.Models;

namespace GGolbik.SecurityTools.Services;

public interface ILoggingService : IDisposable
{
    public Serilog.ILogger ForContext<T>();

    public LogLevel Level { get; set; }

    public void DeleteEntries();

    public IList<LogEntry> GetEntries(LogFilter filter);
}