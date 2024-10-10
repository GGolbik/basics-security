using Microsoft.Data.Sqlite;
using Serilog;
using Serilog.Core;
using Serilog.Events;
namespace GGolbik.SecurityTools.Diagnostic;

public class LoggingService : ILoggingService
{
    private readonly Serilog.ILogger Logger;

    private const string OutputTemplate = "[{Timestamp:yyyy-MM-dd HH:mm:ss.ffffff}] [{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}";

    private readonly LoggingLevelSwitch _levelSwitch;

    public LogLevel Level
    {
        get
        {
            return (LogLevel)_levelSwitch.MinimumLevel;
        }
        set
        {
            _levelSwitch.MinimumLevel = (LogEventLevel)value;
        }
    }

    public LoggingService(LogLevel minimumLogLevel = LogLevel.Information)
    {
        _levelSwitch = new LoggingLevelSwitch((LogEventLevel)minimumLogLevel);

        this.SetupLogger();

        Logger = this.ForContext<LoggingService>();
    }

    public void Dispose()
    {
        Serilog.Log.CloseAndFlush();
    }

    /// <summary>
    /// Setups the serilog based on the configuration.
    /// </summary>
    private void SetupLogger()
    {
        // create logger with level switch
        var loggerConfig = new LoggerConfiguration()
                .MinimumLevel.ControlledBy(_levelSwitch)// ControlledBy must be after readfrom
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information); 

        // add console sink
        this.AddConsoleSink(loggerConfig);

        // add file sink
        this.AddFileSink(loggerConfig);

        // add SQLite sink
        this.AddSqliteSink(loggerConfig);

        // enrich log context
        loggerConfig.Enrich.FromLogContext();

        // set global Serilog Logger
        Log.Logger = loggerConfig.CreateLogger();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="loggerConfig"></param>
    private void AddConsoleSink(LoggerConfiguration loggerConfig)
    {
        loggerConfig.WriteTo.Async(c => c.Console(outputTemplate: OutputTemplate));
    }

    /// <summary>
    /// This will create a file set like:
    /// - securitytools.log
    /// - securitytools_001.log
    /// - securitytools_002.log
    /// </summary>
    /// <param name="loggerConfig"></param>
    private void AddFileSink(LoggerConfiguration loggerConfig)
    {
        // Set log path into app data directory
        string logFile = Path.Combine(Settings.ApplicationDataDirectory, "securitytools.log");

        // create config
        loggerConfig.WriteTo.Async(c => c.File(
            logFile,
            outputTemplate: OutputTemplate,
            rollOnFileSizeLimit: true, // the log file size limit is set to 1GB by default
            retainedFileCountLimit: 8 // Old files will be cleaned up as per retainedFileCountLimit
        ));
    }

    /// <summary>
    /// Adds a sink that writes log events to a SQLite database.
    /// </summary>
    /// <param name="loggerConfig">The logger configuration.</param>
    private void AddSqliteSink(LoggerConfiguration loggerConfig)
    {
        // Set log path into app data directory
        string logFile = Path.Combine(Settings.ApplicationDataDirectory, "securitytools.log.db");

        loggerConfig.WriteTo.Async(c => c.SQLite(
            logFile, // The path of SQLite db.
            tableName: LogDatabase.TableName, // The name of the SQLite table to store log.
            storeTimestampInUtc: true, // Store timestamp in UTC format
            retentionPeriod: null, // The maximum time that a log entry will be kept in the database, or null to disable automatic deletion of old log entries. Non-null values smaller than 30 minute will be replaced with 30 minute.
            retentionCheckInterval: null, // Time period to execute TTL process. Time span should be in 15 minutes increment.
            batchSize: 1, // Number of messages to save as batch to database. Default is 10, max 1000
            maxDatabaseSize: 1000, // Maximum database file size can grow in MB. Default 10 MB, maximum 20 GB
            rollOver: true // If file size grows past max database size, creating rolling backup
        ));
    }

    public Serilog.ILogger ForContext<T>()
    {
        return Log.Logger.ForContext<T>();
    }

    public void DeleteEntries()
    {
        lock (this)
        {
            // A connection string must be provided https://www.connectionstrings.com/sqlite/
            var path = Path.Combine(Settings.ApplicationDataDirectory, "securitytools.log.db");
            var connectionString = $"Data Source={path}";
            using (var sqlite = new SqliteConnection(connectionString)
            {
                DefaultTimeout = 30,
            })
            {
                sqlite.Open();
                new LogDatabase(sqlite).RemoveAll();
            }
        }
    }

    public IList<LogEntry> GetEntries(LogFilter filter)
    {
        lock (this)
        {
            // A connection string must be provided https://www.connectionstrings.com/sqlite/
            var path = Path.Combine(Settings.ApplicationDataDirectory, "securitytools.log.db");
            var connectionString = $"Data Source={path}";
            using (var sqlite = new SqliteConnection(connectionString)
            {
                DefaultTimeout = 30,
            })
            {
                sqlite.Open();
                return new LogDatabase(sqlite).GetEntries(filter);
            }
        }
    }

}
