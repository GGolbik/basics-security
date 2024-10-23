namespace GGolbik.SecurityToolsApp.Diagnostic;

public interface ILoggingService : IDisposable
{
    public Serilog.ILogger ForContext<T>();

    public LogLevel Level { get; set; }

    public void DeleteEntries();

    public IList<LogEntry> GetEntries(LogFilter filter);
}