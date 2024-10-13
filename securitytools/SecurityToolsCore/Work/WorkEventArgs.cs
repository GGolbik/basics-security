namespace GGolbik.SecurityTools.Work;

public class WorkEventArgs : EventArgs
{
    public string Id { get; }
    public string Kind { get; }
    public WorkStatus Status { get; }
    public object? Result { get; }

    public WorkEventArgs(string id, string kind, WorkStatus status, object? result = null)
    {
        this.Id = id;
        this.Kind = kind;
        this.Status = status;
        this.Result = Result;
    }

    internal WorkEventArgs(string id, Work work) : this(id, work.Request.Kind, work.Status, work.Result)
    {

    }
}