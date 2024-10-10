using Microsoft.Data.Sqlite;
using Serilog.Events;

namespace GGolbik.SecurityTools.Diagnostic;

public class LogDatabase
{
    public const string TableName = "Logs";
    private SqliteConnection _sqlite;
    public LogDatabase(SqliteConnection sqlite)
    {
        _sqlite = sqlite;
    }

    private bool IsTableCreated()
    {
        using (var cmd = _sqlite.CreateCommand())
        {
            cmd.CommandText = $"SELECT * FROM sqlite_master WHERE type='table' AND name='{TableName}'";
            var reader = cmd.ExecuteReader();
            return reader.Read();
        }
    }

    public IList<LogEntry> GetEntries(LogFilter filter)
    {
        var result = new List<LogEntry>();
        filter.Since ??= DateTime.MinValue.ToUniversalTime();
        filter.Until ??= DateTime.MaxValue.ToUniversalTime();
        var s = filter.Since?.ToString("yyyy-MM-ddTHH:mm:ss.fff");
        var u = filter.Until?.ToString("yyyy-MM-ddTHH:mm:ss.fff");

        if (!this.IsTableCreated())
        {
            return result;
        }

        using (var cmd = _sqlite.CreateCommand())
        {
            var filterLevels = new HashSet<string>();
            filter.FilterLevel ??= new List<LogEventLevel>();
            foreach (var l in filter.FilterLevel)
            {
                cmd.Parameters.Add(new SqliteParameter("@paramFilter" + l.ToString(), l.ToString()));
                filterLevels.Add(String.Join(" OR ", "Level = @paramFilter" + l.ToString()));
            }
            var filters = new List<string>();
            filters.AddRange(this.GetContainsCondition(cmd, "Properties", filter.FilterProperties ?? new List<string>()));
            filters.AddRange(this.GetContainsCondition(cmd, "RenderedMessage", filter.FilterRenderedMessage ?? new List<string>()));
            filters.AddRange(this.GetContainsCondition(cmd, "Exception", filter.FilterExceptions ?? new List<string>()));
            cmd.Parameters.Add(new SqliteParameter("since", filter.Since?.ToString("yyyy-MM-ddTHH:mm:ss.fff")));
            cmd.Parameters.Add(new SqliteParameter("until", filter.Until?.ToString("yyyy-MM-ddTHH:mm:ss.fff")));
            cmd.CommandText = "SELECT * FROM " + TableName + " WHERE Timestamp between @since AND @until" + (filters.Count() == 0 ? "" : " AND (" + String.Join(" OR ", filters) + ")") + (filterLevels.Count() == 0 ? "" : " AND (" + String.Join(" OR ", filterLevels) + ")") + " ORDER BY Timestamp " + (filter.Direction == SortDirection.Ascending ? "ASC" : "DESC") + ", id " + (filter.Direction == SortDirection.Ascending ? "ASC" : "DESC") + " LIMIT " + (filter.Limit == 0 ? "-1" : filter.Limit) + " OFFSET " + filter.Offset;
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var id = reader.GetInt64(reader.GetOrdinal("id"));
                if(!DateTime.TryParse(reader.GetString(reader.GetOrdinal("Timestamp")) + "Z", out var timestamp))
                {
                    timestamp = DateTime.MinValue.ToUniversalTime();
                }
                LogEventLevel logLevel;
                LogLevel level = LogLevel.None;
                if (Enum.TryParse(reader.GetString(reader.GetOrdinal("Level")) ?? "", true, out logLevel))
                {
                    level = logLevel.ToLogLevel();
                }
                var exception = reader.GetString(reader.GetOrdinal("Exception"));
                var renderedMessage = reader.GetString(reader.GetOrdinal("RenderedMessage"));
                var properties = reader.GetString(reader.GetOrdinal("Properties"));
                result.Add(new LogEntry()
                {
                    Id = id,
                    Timestamp = timestamp,
                    Level = level,
                    Message = renderedMessage,
                    Exception = exception,
                    Properties = properties
                });
            }
        }
        return result;
    }

    private IList<string> GetContainsCondition(SqliteCommand cmd, string column, IList<string> filter)
    {
        var result = new List<string>();
        for (int i = 0; i < filter.Count(); i++)
        {
            string paramName = "@param" + column + i;
            cmd.Parameters.Add(new SqliteParameter(paramName, "%" + filter[i] + "%"));
            result.Add(String.Format("{0} LIKE {1}", column, paramName));
        }
        return result;
    }

    
    /// <summary>
    /// Removes all entries from the database.
    /// </summary>
    /// <returns>The number of deleted entries</returns>
    public int RemoveAll()
    {
        using (var cmd = _sqlite.CreateCommand())
        {
            cmd.CommandText = "DELETE FROM " + TableName;
            int affectedRows = cmd.ExecuteNonQuery();
            return affectedRows;
        }
    }
}

internal static class LogEventLevelExtension
{
    public static LogLevel ToLogLevel(this LogEventLevel logLevel)
    {
        switch (logLevel)
        {
            case LogEventLevel.Verbose:
                return LogLevel.Trace;
            case LogEventLevel.Debug:
                return LogLevel.Debug; ;
            case LogEventLevel.Information:
                return LogLevel.Information;
            case LogEventLevel.Warning:
                return LogLevel.Warning;
            case LogEventLevel.Error:
                return LogLevel.Error;
            default:
                return LogLevel.Critical;
        }
    }
}