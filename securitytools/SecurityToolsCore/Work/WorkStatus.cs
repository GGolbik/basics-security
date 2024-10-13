
namespace GGolbik.SecurityTools.Work;

public class WorkStatus : ICloneable
{
    public WorkState State { get; set; }

    /// <summary>
    /// The time when the request has been enqueued.
    /// </summary>
    public DateTime? Enqueued { get; set; }

    /// <summary>
    /// The time when the request execution started.
    /// </summary>
    public DateTime? ExecutionStart { get; set; }

    /// <summary>
    /// The time when the request execution stopped/finished.
    /// </summary>
    public DateTime? ExecutionEnd { get; set; }

    /// <summary>
    /// Cannot caluclated by the diff of Start and End because the time could be changed between.
    /// Use Environment.TickCount64 to keep track of duration.
    /// </summary>
    public TimeSpan? ExecutionDuration { get; set; }

    /// <summary>
    /// Might contain an unexpected error during execution.
    /// </summary>
    public Exception? Error { get; set; }

    public WorkStatus()
    {

    }

    public object Clone()
    {
        return this.MemberwiseClone();
    }
}