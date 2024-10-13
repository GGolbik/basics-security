
namespace GGolbik.SecurityTools.Work;

internal class Work
{
    public WorkRequest Request { get; }
    public WorkStatus Status { get; set; }
    public object? Result { get; set; }
    public CancellationTokenSource TokenSource { get; } = new();
    public CancellationTokenSource? CombinedTokenSource { get; set; }
    public CancellationToken Token { get => CombinedTokenSource?.Token ?? TokenSource.Token; }
    public Task? WorkTask { get; set; }
    public Action<Task> WorkAction { get; set; } = (_) => {};

    public Work(WorkRequest request) : this(request, new())
    {
    }

    public Work(WorkRequest request, WorkStatus status)
    {
        this.Request = request;
        this.Status = status;
    }
}
