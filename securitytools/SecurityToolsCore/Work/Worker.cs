
using System.Diagnostics.CodeAnalysis;

namespace GGolbik.SecurityTools.Work;

using WorkHandler = Func<WorkRequest, CancellationToken, object?>;

public class Worker : IWorker
{
    /// <summary>
    /// Whether the worker is started.
    /// </summary>
    private bool _enabled = false;

    /// <summary>
    /// The cancel token, if the worker shall be stopped.
    /// </summary>
    private CancellationTokenSource _workerCancelToken = new();

    /// <summary>
    /// The registered handlers.
    /// </summary>
    private readonly Dictionary<string, WorkHandler> _handlers = new();

    #region Work
    /// <summary>
    /// The enqueued work.
    /// </summary>
    private readonly Dictionary<string, Work> _work = new();
    #endregion

    #region notifications
    /// <summary>
    /// A queue which contains notifications to be send.
    /// </summary>
    private readonly Queue<WorkEventArgs> _notifications = new();

    /// <summary>
    /// The event is used to inform about a new notification to be send.
    /// </summary>
    private readonly AutoResetEvent _notifyEvent = new(false);

    /// <summary>
    /// The cancel token, if the notifier shall be stopped.
    /// </summary>
    private CancellationTokenSource _notifyCancelToken = new();

    /// <summary>
    /// The task which sends notifications.
    /// </summary>
    private Task _notifyTask;
    #endregion

    public event EventHandler<WorkEventArgs>? OnStateChanged;

    public Worker() : this(false)
    {
    }

    public Worker(bool start)
    {
        _notifyTask = this.CreateNotifyTask(_notifyCancelToken.Token);
        _notifyTask.Start();
        if (start)
        {
            this.Start();
        }
    }

    public void Dispose()
    {
        this.Stop();
        _notifyCancelToken.Cancel();
        try
        {
            _notifyTask.Wait();
        }
        catch
        {
            // ignore task canceled exceptions
        }
    }

    public void Start()
    {
        lock (this)
        {
            if (_enabled)
            {
                return;
            }
            _enabled = true;
            _workerCancelToken = new();
            foreach (var entry in _work)
            {
                if (entry.Value.Status.State != WorkState.Waiting)
                {
                    continue;
                }
                try
                {
                    var delay = this.GetDelay(entry.Value);
                    this.StartWork(entry.Value, delay);
                }
                catch (Exception e)
                {
                    this.Notify(entry.Key, WorkState.Error, null, e);
                }
            }
        }
    }

    public void Stop()
    {
        lock (this)
        {
            if (!_enabled)
            {
                return;
            }
            _enabled = false;
            _workerCancelToken.Cancel();
            List<Task> tasks = new();
            lock (_work)
            {
                foreach (var work in _work.Values)
                {
                    if (work.WorkTask == null)
                    {
                        continue;
                    }
                    tasks.Add(work.WorkTask);
                    work.TokenSource.Cancel();
                }
            }
            try
            {
                Task.WaitAll(tasks.ToArray());
            }
            catch
            {
                // ignore task canceled exceptions
            }
        }
    }

    public WorkHandler? SetRequestHandler(string kind, WorkHandler handler)
    {
        lock (_handlers)
        {
            if (string.IsNullOrEmpty(kind))
            {
                throw new ArgumentException($"Invalid request kind '{kind}'");
            }
            _handlers.Remove(kind, out var previous);
            _handlers.Add(kind, handler);
            return previous;
        }
    }

    public bool RemoveRequestHandler(string kind)
    {
        lock (_handlers)
        {
            return _handlers.Remove(kind);
        }
    }

    public string Enqueue(WorkRequest request)
    {
        lock (this) lock (_handlers) lock (_work)
                {
                    request = (WorkRequest)request.Clone();
                    if (string.IsNullOrEmpty(request.Kind) || !_handlers.ContainsKey(request.Kind))
                    {
                        throw new ArgumentException($"There is no handler for the request kind '{request.Kind}'");
                    }
                    string id = Guid.NewGuid().ToString();
                    Work work = new Work(request);
                    work.CombinedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_workerCancelToken.Token, work.Token);
                    work.WorkAction = this.CreateWorkAction(id, work);
                    var delay = this.GetDelay(work);
                    _work.Add(id, work);
                    work.Status.Enqueued = DateTime.UtcNow;
                    this.Notify(id, WorkState.Waiting);
                    if (_enabled)
                    {
                        try
                        {
                            this.StartWork(work, delay);
                        }
                        catch (Exception e)
                        {
                            this.Notify(id, WorkState.Error, null, e);
                        }
                    }
                    return id;
                }
    }

    public IDictionary<string, WorkRequest> GetRequests()
    {
        lock (_work)
        {
            Dictionary<string, WorkRequest> result = new();
            foreach (var entry in _work)
            {
                result.Add(entry.Key, (WorkRequest)entry.Value.Request.Clone());
            }
            return result;
        }
    }

    public IDictionary<string, WorkStatus> GetStatus()
    {
        lock (_work)
        {
            Dictionary<string, WorkStatus> result = new();
            foreach (var entry in _work)
            {
                result.Add(entry.Key, (WorkStatus)entry.Value.Status.Clone());
            }
            return result;
        }
    }

    public bool TryGetStatus(string id, [NotNullWhen(true)] out WorkStatus? status)
    {
        lock (_work)
        {
            if (_work.TryGetValue(id, out var work))
            {
                status = work.Status;
                return true;
            }
            status = null;
            return false;
        }
    }

    public bool TryGetResult(string id, [NotNullWhen(true)] out WorkStatus? status, out object? result)
    {
        return this.TryGetResult(id, TimeSpan.Zero, out status, out result);
    }

    public bool TryGetResult(string id, TimeSpan timeout, [NotNullWhen(true)] out WorkStatus? status, out object? result)
    {
        AutoResetEvent changeEvent = new(false);
        EventHandler<WorkEventArgs> eventHandler = (_, args) =>
        {
            if (!string.Equals(id, args.Id))
            {
                return;
            }
            switch (args.Status.State)
            {
                case WorkState.Error:
                case WorkState.Done:
                    changeEvent.Set();
                    break;
                default:
                    break;
            }
        };

        lock (_work)
        {
            if (_work.TryGetValue(id, out var work))
            {
                eventHandler.Invoke(this, new WorkEventArgs(id, work));
            }
            else
            {
                // unknown id
                status = null;
                result = null;
                return false;
            }
            // register event
            this.OnStateChanged += eventHandler;
        }

        bool hasResult;
        try
        {
            hasResult = changeEvent.WaitOne(timeout > TimeSpan.Zero ? timeout : TimeSpan.Zero);
        }
        catch
        {
            throw;
        }
        finally
        {
            this.OnStateChanged -= eventHandler;
        }
        if (hasResult)
        {
            lock (_work)
            {
                if (_work.Remove(id, out var work))
                {
                    status = work.Status;
                    result = work.Result;
                    return true;
                }
            }
        } // else timeout

        status = null;
        result = null;
        return false;
    }

    public void Cancel(string id)
    {
        lock (_work)
        {
            if (_work.TryGetValue(id, out var work))
            {
                if (work.TokenSource.IsCancellationRequested)
                {
                    return;
                }
                work.TokenSource.Cancel();
                if (work.Status.State != WorkState.Waiting)
                {
                    return;
                }
                if (work.WorkTask == null || work.WorkTask.Status != TaskStatus.WaitingForChildrenToComplete)
                {
                    this.Notify(id, WorkState.Error, null, new OperationCanceledException());
                }
            }
        }
    }

    private TimeSpan GetDelay(Work work)
    {
        TimeSpan delay = work.Request.NotBefore.ToUniversalTime() - DateTime.UtcNow;
        if (delay < TimeSpan.Zero)
        {
            delay = TimeSpan.Zero;
        }
        var token = new CancellationTokenSource();
        var task = Task.Delay(delay, token.Token);
        token.Cancel();
        return delay;
    }

    private Action<Task> CreateWorkAction(string id, Work work)
    {
        return (Task t) =>
        {
            if (t.IsCanceled)
            {
                this.Notify(id, WorkState.Error, null, new OperationCanceledException());
                return;
            }
            long ticks = Environment.TickCount64;
            lock (_work)
            {
                work.Status.ExecutionStart = DateTime.UtcNow;
            }
            WorkHandler? handler;
            lock (_handlers)
            {
                if (!_handlers.TryGetValue(work.Request.Kind, out handler))
                {
                    this.Notify(id, WorkState.Error, null, new NotSupportedException("There is no handler for this task."));
                    return;
                }
            }
            WorkState workState = WorkState.Error;
            Exception? error = null;
            object? result = null;
            try
            {
                this.Notify(id, WorkState.Executing);
                result = handler.Invoke(work.Request, work.Token);
                workState = WorkState.Done;
            }
            catch (Exception e)
            {
                error = e;
            }
            finally
            {
                lock (_work)
                {
                    work.Status.ExecutionEnd = DateTime.UtcNow;
                    work.Status.ExecutionDuration = TimeSpan.FromMilliseconds(Environment.TickCount64 - ticks);
                    this.Notify(id, workState, result, error);
                }
            }
        };
    }

    private void StartWork(Work work, TimeSpan delay)
    {
        if (work.Request.Timeout > TimeSpan.Zero)
        {
            work.TokenSource.CancelAfter(work.Request.Timeout);
        }
        work.WorkTask = Task.Delay(delay, work.Token).ContinueWith(work.WorkAction, work.Token);
    }

    private Task CreateNotifyTask(CancellationToken token)
    {
        token.Register(() =>
        {
            _notifyEvent.Set();
        });
        return new Task(() =>
        {
            while (true)
            {
                lock (_notifications)
                {
                    if (_notifications.Count() > 0)
                    {
                        // signal as long as there are notifications
                        _notifyEvent.Set();
                    }
                }
                _notifyEvent.WaitOne();
                token.ThrowIfCancellationRequested();
                WorkEventArgs? args;
                lock (_notifications)
                {
                    if (!_notifications.TryDequeue(out args))
                    {
                        continue;
                    }
                }
                this.OnStateChanged?.Invoke(this, args);
            }
        }, token);
    }

    private void Notify(string id, WorkState state, object? result = null, Exception? error = null)
    {
        lock (_work)
            lock (_notifications)
            {
                if (!_work.TryGetValue(id, out var work))
                {
                    return;
                }
                if (work.Status.State >= state)
                {
                    // invalid transition
                    return;
                }
                if (error != null)
                {
                    work.Status.Error = error;
                }
                work.Status.State = state;
                work.Result = result;
                _notifications.Enqueue(new WorkEventArgs(id, work.Request.Kind, (WorkStatus)work.Status.Clone(), result));
                _notifyEvent.Set();
            }
    }
}