
namespace GGolbik.SecurityTools.Work;

public class WorkRequest : ICloneable
{
    /// <summary>
    /// Identifies the kind of request which will be used to identify the matching request handler.
    /// </summary>
    public string Kind { get; }

    /// <summary>
    /// The user data of the request.
    /// </summary>
    public object? Data { get; set; }

    /// <summary>
    /// An optional time to define
    /// </summary>
    public DateTime NotBefore { get; set; } = DateTime.MinValue;

    /// <summary>
    /// The max duration of execution.
    /// </summary>
    public TimeSpan Timeout { get; set; } = TimeSpan.Zero;

    public WorkRequest(string kind, object? data = null)
    {
        this.Kind = kind;
        this.Data = data;
    }

    public object Clone()
    {
        return this.MemberwiseClone();
    }
}