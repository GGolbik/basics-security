
using System.Diagnostics.CodeAnalysis;

namespace GGolbik.SecurityTools.Work;

using WorkHandler = Func<WorkRequest, CancellationToken, object?>;

/// <summary>
/// A worker which executes requests.
/// </summary>
public interface IWorker : IDisposable
{
    /// <summary>
    /// Starts the worker.
    /// </summary>
    void Start();

    /// <summary>
    /// Stops the worker and cancels already running tasks.
    /// </summary>
    void Stop();

    /// <summary>
    /// Sets the request handler for the specified kind.
    /// </summary>
    /// <param name="kind">The request kind to handle.</param>
    /// <param name="handler">The handler.</param>
    /// <returns>The replaced handler.</returns>
    /// <exception cref="ArgumentException">If the kind is invalid.</exception>
    WorkHandler? SetRequestHandler(string kind, WorkHandler handler);

    /// <summary>
    /// Removes the handler of the kind.
    /// </summary>
    /// <param name="kind">The request kind.</param>
    /// <returns>True if there was a handler to remove.</returns>
    bool RemoveRequestHandler(string kind);

    /// <summary>
    /// Enqueues a request.
    /// </summary>
    /// <param name="request">The task to enqueue.</param>
    /// <returns>The ID of the request.</returns>
    /// <exception cref="Exception">If the task could not be enqueued.</exception>
    string Enqueue(WorkRequest request);

    /// <summary>
    /// Returns the status of all requests.
    /// </summary>
    /// <returns></returns>
    IDictionary<string, WorkRequest> GetRequests();

    /// <summary>
    /// Returns the status of all requests.
    /// </summary>
    /// <returns>All status information.</returns>
    IDictionary<string, WorkStatus> GetStatus();

    /// <summary>
    /// Tries to get the status.
    /// </summary>
    /// <param name="id">The ID of the request.</param>
    /// <param name="status">The status.</param>
    /// <returns>True if the status could be found.</returns>
    bool TryGetStatus(string id, [NotNullWhen(true)] out WorkStatus? status);

    /// <summary>
    /// Tries to get the result. Will remove the status from the worker.
    /// </summary>
    /// <param name="id">The ID of the request.</param>
    /// <param name="status">The status.</param>
    /// <param name="result">The result.</param>
    /// <returns>True if the response is available.</returns>
    bool TryGetResult(string id, [NotNullWhen(true)] out WorkStatus? status, out object? result);

    /// <summary>
    /// Tries to get a response. Waits until the request has been processed or canceled.
    /// </summary>
    /// <param name="id">The ID of the request.</param>
    /// <param name="timeout">The max time to wait.</param>
    /// <param name="status">The status.</param>
    /// <param name="result">The result.</param>
    /// <returns>True if the response is available.</returns>
    bool TryGetResult(string id, TimeSpan timeout, [NotNullWhen(true)] out WorkStatus? status, out object? result);

    /// <summary>
    /// Cancels a request.
    /// You need to check the response to know whether the request has been canceled or has already been finished.
    /// </summary>
    /// <param name="id">The ID of the request.</param>
    void Cancel(string id);

    /// <summary>
    /// Raises an event if the state changes of a request.
    /// </summary>
    event EventHandler<WorkEventArgs>? OnStateChanged;
}
