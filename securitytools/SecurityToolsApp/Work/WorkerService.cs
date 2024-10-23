
using System.Diagnostics.CodeAnalysis;
using GGolbik.SecurityTools.Work;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace GGolbik.SecurityToolsApp.Work;

public class WorkerService : IWorkerService
{
    private readonly Serilog.ILogger Logger = Log.ForContext<WorkerService>();
    private Worker _worker = new(true);
    private Queue<WorkEventArgs> _events = new(25);

    #pragma warning disable CS0067
    public event EventHandler<WorkEventArgs>? OnStateChanged;
    #pragma warning restore CS0067

    public WorkerService()
    {
        _worker.OnStateChanged += (_, args) =>
        {
            Logger.Debug("{0}: {1}", args.Kind, args.Status.State);
            lock(_events)
            {
                _events.Enqueue(args);
            }
        };
    }

    public void Dispose()
    {
        _worker.Dispose();
    }

    public Worker GetWorker()
    {
        return _worker;
    }

    public IActionResult GetResponse(WorkRequest request)
    {
        return this.GetResponse(request, TimeSpan.FromMinutes(5));
    }

    public IActionResult GetResponse(WorkRequest request, TimeSpan timeout)
    {
        if (request.Timeout <= TimeSpan.Zero)
        {
            request.Timeout = timeout;
        }
        var result = this.GetResult(request, timeout);
        return (IActionResult)(result ?? throw new InvalidOperationException());
    }

    public object? GetResult(WorkRequest request)
    {
        return this.GetResult(request, TimeSpan.FromMinutes(5));
    }

    public object? GetResult(WorkRequest request, TimeSpan timeout)
    {
        if (request.Timeout <= TimeSpan.Zero)
        {
            request.Timeout = timeout;
        }
        var id = _worker.Enqueue(request);
        _worker.TryGetResult(id, timeout, out var status, out var result);
        if(status?.Error != null)
        {
            throw status.Error;
        }
        return result;
    }

    public IList<WorkEventArgs> GetEvents()
    {
        lock(_events)
        {
            var result = _events.ToList();
            result.Reverse();
            return result;
        }
    }
    public void Start()
    {
        throw new NotSupportedException();
    }

    public void Stop()
    {
        throw new NotSupportedException();
    }

    public Func<WorkRequest, CancellationToken, object?>? SetRequestHandler(string kind, Func<WorkRequest, CancellationToken, object?> handler)
    {
        return _worker.SetRequestHandler(kind, handler);
    }

    public bool RemoveRequestHandler(string kind)
    {
        return _worker.RemoveRequestHandler(kind);
    }

    public string Enqueue(WorkRequest request)
    {
        return _worker.Enqueue(request);
    }

    public IDictionary<string, WorkRequest> GetRequests()
    {
        return _worker.GetRequests();
    }

    public IDictionary<string, WorkStatus> GetStatus()
    {
        return _worker.GetStatus();
    }

    public bool TryGetStatus(string id, [NotNullWhen(true)] out WorkStatus? status)
    {
        return _worker.TryGetStatus(id, out status);
    }

    public bool TryGetResult(string id, [NotNullWhen(true)] out WorkStatus? status, out object? result)
    {
        return _worker.TryGetResult(id, out status, out result);
    }

    public bool TryGetResult(string id, TimeSpan timeout, [NotNullWhen(true)] out WorkStatus? status, out object? result)
    {
        return _worker.TryGetResult(id, timeout, out status, out result);
    }

    public void Cancel(string id)
    {
        _worker.Cancel(id);
    }
}