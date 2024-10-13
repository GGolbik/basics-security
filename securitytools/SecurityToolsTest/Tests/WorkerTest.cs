using GGolbik.SecurityTools.Work;

namespace GGolbik.SecurityToolsTest;


public class WorkerTest
{

    [Fact]
    public void Test_Enqueue_No_Handler_Error()
    {
        using(Worker worker = new())
        {
            Action act = () => worker.Enqueue(new WorkRequest("test"));
            act.Should().Throw<ArgumentException>("no handler").WithMessage("*no handler*");
        }
    }

    [Fact]
    public void Test_Enqueue_No_Kind_Error()
    {
        using(Worker worker = new())
        {
            Action act = () => worker.Enqueue(new WorkRequest(""));
            act.Should().Throw<ArgumentException>("no kind").WithMessage("*no handler*");
        }
    }

    [Fact]
    public void Test_SetRequestHandler_Error()
    {
        using(Worker worker = new())
        {
            Action act = () => worker.SetRequestHandler("", (_, _) => { return null; });
            act.Should().Throw<ArgumentException>("no kind").WithMessage("Invalid request kind*");
        }
    }

    [Fact]
    public void Test_SetRequestHandler_Success()
    {
        using(Worker worker = new())
        {
            Func<WorkRequest, CancellationToken, object?> e1 = (_, _) => { return null; };
            Func<WorkRequest, CancellationToken, object?> e2 = (_, _) => { return null; };
            worker.SetRequestHandler(nameof(WorkerTest), e1).Should().BeNull();
            worker.SetRequestHandler(nameof(WorkerTest), e2).Should().Be(e1);
        }
    }

    [Fact]
    public void Test_RemoveRequestHandler()
    {
        using(Worker worker = new())
        {
            worker.RemoveRequestHandler(nameof(WorkerTest)).Should().BeFalse();
            worker.SetRequestHandler(nameof(WorkerTest), (_, _) => { return null; });
            worker.RemoveRequestHandler(nameof(WorkerTest)).Should().BeTrue();
        }
    }

    [Fact]
    public void Test_GetRequests()
    {
        using(Worker worker = new())
        {
            worker.SetRequestHandler(nameof(WorkerTest), (_, _) => { 
                return null;
            });
            WorkRequest request = new WorkRequest(nameof(WorkerTest));
            string id = worker.Enqueue(request);
            var requests = worker.GetRequests();
            requests.Should().NotBeEmpty();
            requests.Should().ContainKey(id);
            if(requests.TryGetValue(id, out var item))
            {
                item.Kind.Should().Be(nameof(WorkerTest));
                item.Data.Should().Be(request.Data);
            }
        }
    }

    [Fact]
    public void Test_GetStatus()
    {
        using(Worker worker = new())
        {
            worker.SetRequestHandler(nameof(WorkerTest), (_, _) => { 
                return null;
            });
            WorkRequest request = new WorkRequest(nameof(WorkerTest));
            string id = worker.Enqueue(request);
            var status = worker.GetStatus();
            status.Should().NotBeEmpty();
            status.Should().ContainKey(id);
            if(status.TryGetValue(id, out var item))
            {
                item.State.Should().Be(WorkState.Waiting);
                item.Enqueued.Should().BeBefore(DateTime.UtcNow);
            }
        }
    }

    [Fact]
    public void Test_TryGetStatus()
    {
        using(Worker worker = new())
        {
            worker.TryGetStatus("", out var status).Should().BeFalse();
            status.Should().BeNull();
            worker.SetRequestHandler(nameof(WorkerTest), (_, _) => { 
                return null;
            });
            WorkRequest request = new WorkRequest(nameof(WorkerTest));
            string id = worker.Enqueue(request);
            var hasStatus = worker.TryGetStatus(id, out var item);
            hasStatus.Should().BeTrue();
            if(hasStatus)
            {
                item!.State.Should().Be(WorkState.Waiting);
                item.Enqueued.Should().BeBefore(DateTime.UtcNow);
            }
        }
    }

    [Fact]
    public void Test_Enqueue_Running_Success()
    {
        using(Worker worker = new(true))
        {
            bool called = false;
            List<WorkEventArgs> eventArgs = new();
            worker.SetRequestHandler(nameof(WorkerTest), (_, _) => { 
                called = true; return null;
            });
            worker.OnStateChanged += (_, args) => { eventArgs.Add(args); };
            string id = worker.Enqueue(new WorkRequest(nameof(WorkerTest)) { Timeout = TimeSpan.FromMinutes(5) });
            var hasResult = worker.TryGetResult(id, TimeSpan.FromMinutes(5), out var status, out var result);
            hasResult.Should().BeTrue();
            Thread.Sleep(100); // wait for all events
            called.Should().BeTrue();
            eventArgs.Count().Should().Be(3);
            if(eventArgs.Count() == 3)
            {
                eventArgs[0].Kind.Should().Be(nameof(WorkerTest));
                eventArgs[0].Status.State.Should().Be(WorkState.Waiting);
                eventArgs[1].Kind.Should().Be(nameof(WorkerTest));
                eventArgs[1].Status.State.Should().Be(WorkState.Executing);
                eventArgs[2].Kind.Should().Be(nameof(WorkerTest));
                eventArgs[2].Status.State.Should().Be(WorkState.Done);
            }
        }
    }

    [Fact]
    public void Test_Enqueue_StoppedSuccess()
    {
        using(Worker worker = new(false))
        {
            bool called = false;
            List<WorkEventArgs> eventArgs = new();
            worker.SetRequestHandler(nameof(WorkerTest), (_, _) => { 
                called = true; return null;
            });
            worker.OnStateChanged += (_, args) => { eventArgs.Add(args); };
            string id = worker.Enqueue(new WorkRequest(nameof(WorkerTest)) { Timeout = TimeSpan.FromMinutes(5) });
            var hasResult = worker.TryGetResult(id, out var status, out var result);
            hasResult.Should().BeFalse();
            Thread.Sleep(100); // wait for all events
            called.Should().BeFalse();
            eventArgs.Count().Should().Be(1);
            if(eventArgs.Count() == 1)
            {
                eventArgs[0].Kind.Should().Be(nameof(WorkerTest));
                eventArgs[0].Status.State.Should().Be(WorkState.Waiting);
            }
            worker.Start();
            hasResult = worker.TryGetResult(id, TimeSpan.FromMinutes(5), out status, out result);
            hasResult.Should().BeTrue();
            Thread.Sleep(100); // wait for all events
            called.Should().BeTrue();
            eventArgs.Count().Should().Be(3);
            if(eventArgs.Count() == 3)
            {
                eventArgs[1].Kind.Should().Be(nameof(WorkerTest));
                eventArgs[1].Status.State.Should().Be(WorkState.Executing);
                eventArgs[2].Kind.Should().Be(nameof(WorkerTest));
                eventArgs[2].Status.State.Should().Be(WorkState.Done);
            }
        }
    }

    [Fact]
    public void Test_Cancel_Waiting()
    {
        using(Worker worker = new(true))
        {
            bool called = false;
            List<WorkEventArgs> eventArgs = new();
            worker.SetRequestHandler(nameof(WorkerTest), (_, _) => { 
                called = true; return null;
            });
            worker.OnStateChanged += (_, args) => { eventArgs.Add(args); };
            string id = worker.Enqueue(new WorkRequest(nameof(WorkerTest)) { Timeout = TimeSpan.FromMinutes(5), NotBefore = DateTime.Now.AddMinutes(10) });
            worker.Cancel(id);
            Thread.Sleep(100); // wait for all events
            called.Should().BeFalse();
            eventArgs.Count().Should().Be(2);
            if(eventArgs.Count() == 2)
            {
                eventArgs[0].Kind.Should().Be(nameof(WorkerTest));
                eventArgs[0].Status.State.Should().Be(WorkState.Waiting);
                eventArgs[1].Kind.Should().Be(nameof(WorkerTest));
                eventArgs[1].Status.State.Should().Be(WorkState.Error);
                eventArgs[1].Status.Error.Should().NotBeNull();
            }
        }
    }

    [Fact]
    public void Test_Cancel_Running()
    {
        using(Worker worker = new(true))
        {
            bool called = false;
            List<WorkEventArgs> eventArgs = new();
            worker.SetRequestHandler(nameof(WorkerTest), (_, t) => { 
                called = true;
                Task.Delay(TimeSpan.FromMinutes(5), t).Wait();
                return null;
            });
            worker.OnStateChanged += (_, args) => { eventArgs.Add(args); };
            string id = worker.Enqueue(new WorkRequest(nameof(WorkerTest)) { Timeout = TimeSpan.FromMinutes(5) });
            Thread.Sleep(1000); // wait for task to run
            worker.Cancel(id);
            Thread.Sleep(100); // wait for all events
            called.Should().BeTrue();
            eventArgs.Count().Should().Be(3);
            if(eventArgs.Count() == 3)
            {
                eventArgs[0].Kind.Should().Be(nameof(WorkerTest));
                eventArgs[0].Status.State.Should().Be(WorkState.Waiting);
                eventArgs[1].Kind.Should().Be(nameof(WorkerTest));
                eventArgs[1].Status.State.Should().Be(WorkState.Executing);
                eventArgs[2].Kind.Should().Be(nameof(WorkerTest));
                eventArgs[2].Status.State.Should().Be(WorkState.Error);
                eventArgs[2].Status.Error.Should().NotBeNull();
            }
        }
    }

    [Fact]
    public void Test_Notify_AfterRestartWithFinishedTasks()
    {
        using(Worker worker = new(true))
        {
            bool called = false;
            List<WorkEventArgs> eventArgs = new();
            worker.SetRequestHandler(nameof(WorkerTest), (_, _) => { 
                called = true; return null;
            });
            worker.OnStateChanged += (_, args) => { eventArgs.Add(args); };
            string id = worker.Enqueue(new WorkRequest(nameof(WorkerTest)) { Timeout = TimeSpan.FromMinutes(5), NotBefore = DateTime.Now.AddMinutes(10) });
            worker.Cancel(id);
            Thread.Sleep(100); // wait for all events
            called.Should().BeFalse();
            eventArgs.Count().Should().Be(2);
            if(eventArgs.Count() == 2)
            {
                eventArgs[0].Kind.Should().Be(nameof(WorkerTest));
                eventArgs[0].Status.State.Should().Be(WorkState.Waiting);
                eventArgs[1].Kind.Should().Be(nameof(WorkerTest));
                eventArgs[1].Status.State.Should().Be(WorkState.Error);
                eventArgs[1].Status.Error.Should().NotBeNull();
            }
            worker.Stop();
            worker.Start();
            Thread.Sleep(100); // wait for all events
            eventArgs.Count().Should().Be(2);
        }
    }
}