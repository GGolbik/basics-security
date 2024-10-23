using System.Reflection;
using GGolbik.SecurityTools.Work;
using GGolbik.SecurityToolsApp.Work;
using Microsoft.AspNetCore.Mvc;

namespace GGolbik.SecurityToolsApp.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class WorkerController : ControllerBase
{
    private readonly ILogger<WorkerController> _logger;
    private readonly IWorkerService _workerService;

    public WorkerController(ILogger<WorkerController> logger, IWorkerService workerService)
    {
        _logger = logger;
        _workerService = workerService;
    }

    /// <summary>
    /// Returns the requests.
    /// </summary>
    /// <returns>The status.</returns>
    /// <response code="200">The 200 (OK) status code indicates that the request has succeeded. The payload contains the requests.</response>
    /// <response code="500">The 500 (Internal Server Error) status code indicates that the server encountered an unexpected condition that prevented it from fulfilling the request.</response>
    [ProducesResponseType(typeof(IDictionary<string, WorkRequest>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [HttpGet("requests")]
    public IActionResult GetRequests([FromQuery] int? limit = null)
    {
        if (limit == null || limit < 0)
        {
            limit = 25;
        }
        _logger.LogTrace(MethodBase.GetCurrentMethod()?.Name);
        return Ok(_workerService.GetRequests().Take((int)limit));
    }

    /// <summary>
    /// Returns the status.
    /// </summary>
    /// <returns>The status.</returns>
    /// <response code="200">The 200 (OK) status code indicates that the request has succeeded. The payload contains the status.</response>
    /// <response code="500">The 500 (Internal Server Error) status code indicates that the server encountered an unexpected condition that prevented it from fulfilling the request.</response>
    [ProducesResponseType(typeof(IDictionary<string, WorkStatus>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [HttpGet("status")]
    public IActionResult GetStatus()
    {
        _logger.LogTrace(MethodBase.GetCurrentMethod()?.Name);
        return Ok(_workerService.GetStatus());
    }

    /// <summary>
    /// Returns the status.
    /// </summary>
    /// <param name="requestId">The ID of the request.</param>
    /// <returns>The status.</returns>
    /// <response code="200">The 200 (OK) status code indicates that the request has succeeded. The payload contains the status.</response>
    /// <response code="400">The 400 (Bad Request) status code indicates that the server cannot or will not process the request due to something that is perceived to be a client error (for example, malformed request syntax, invalid request message framing, or deceptive request routing).</response>
    /// <response code="500">The 500 (Internal Server Error) status code indicates that the server encountered an unexpected condition that prevented it from fulfilling the request.</response>
    [ProducesResponseType(typeof(WorkStatus), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [HttpGet("requests/{requestId}/status")]
    public IActionResult GetStatus([FromRoute] string requestId)
    {
        _logger.LogTrace(MethodBase.GetCurrentMethod()?.Name);
        if (_workerService.TryGetStatus(requestId, out var status))
        {
            return Ok(status);
        }
        return NotFound();
    }

    /// <summary>
    /// Returns the status.
    /// </summary>
    /// <param name="requestId">The ID of the request.</param>
    /// <param name="timeout">The timeout in seconds. Default is 0.</param>
    /// <returns>The status.</returns>
    /// <response code="200">The 200 (OK) status code indicates that the request has succeeded. The payload contains the status.</response>
    /// <response code="400">The 400 (Bad Request) status code indicates that the server cannot or will not process the request due to something that is perceived to be a client error (for example, malformed request syntax, invalid request message framing, or deceptive request routing).</response>
    /// <response code="406">The 406 (Not Acceptable) status code is sent when the web server, after performing server-driven content negotiation, doesn't find any content that conforms to the criteria given by the user agent.</response>
    /// <response code="500">The 500 (Internal Server Error) status code indicates that the server encountered an unexpected condition that prevented it from fulfilling the request.</response>
    [ProducesResponseType(typeof(WorkStatus), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [HttpGet("requests/{requestId}/result")]
    public IActionResult GetResult([FromRoute] string requestId, [FromQuery] uint timeout = 0)
    {
        _logger.LogTrace(MethodBase.GetCurrentMethod()?.Name);
        if (!_workerService.TryGetStatus(requestId, out var status))
        {
            return NotFound();
        }
        if (_workerService.TryGetResult(requestId, TimeSpan.FromSeconds(timeout), out status, out var result))
        {
            if (status.State == WorkState.Error)
            {
                throw status.Error ?? new NullReferenceException(nameof(WorkStatus.Error));
            }
            return Ok(result);
        }
        return StatusCode(StatusCodes.Status406NotAcceptable);
    }

    /// <summary>
    /// Returns the events.
    /// </summary>
    /// <returns>The events.</returns>
    /// <response code="200">The 200 (OK) status code indicates that the request has succeeded. The payload contains the events.</response>
    /// <response code="500">The 500 (Internal Server Error) status code indicates that the server encountered an unexpected condition that prevented it from fulfilling the request.</response>
    [ProducesResponseType(typeof(IList<WorkEventArgs>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [HttpGet("events")]
    public IActionResult GetEvents()
    {
        _logger.LogTrace(MethodBase.GetCurrentMethod()?.Name);
        return Ok(_workerService.GetEvents());
    }

    /// <summary>
    /// Adds a new request.
    /// </summary>
    /// <param name="kind">The request kind.</param>
    /// <returns>The ID of the new configuration.</returns>
    /// <response code="200">The 200 (OK) status code indicates that the request has succeeded. The payload contains the ID of the created connection configuration.</response>
    /// <response code="400">The 400 (Bad Request) status code indicates that the server cannot or will not process the request due to something that is perceived to be a client error (for example, malformed request syntax, invalid request message framing, or deceptive request routing).</response>
    /// <response code="500">The 500 (Internal Server Error) status code indicates that the server encountered an unexpected condition that prevented it from fulfilling the request.</response>
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [HttpPost("requests/{kind}")]
    public IActionResult Enqueue([FromRoute] string kind)
    {
        _logger.LogTrace(MethodBase.GetCurrentMethod()?.Name);
        WorkRequest request = new(kind, this.HttpContext);
        return Ok(_workerService.Enqueue(request));
    }

    /// <summary>
    /// Cancels an existing request. You must still read the result to remove it from the cache.
    /// </summary>
    /// <param name="requestId">The ID of the request.</param>
    /// <response code="204">The 204 (No Content) status code indicates that the server has successfully fulfilled the request and that there is no content to send in the response payload body.</response>
    /// <response code="500">The 500 (Internal Server Error) status code indicates that the server encountered an unexpected condition that prevented it from fulfilling the request.</response>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [HttpDelete("requests/{requestId}")]
    public IActionResult Cancel([FromRoute] string requestId)
    {
        _logger.LogTrace(MethodBase.GetCurrentMethod()?.Name);
        _workerService.Cancel(requestId);
        return NoContent();
    }
}
