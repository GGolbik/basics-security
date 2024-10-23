using System.Reflection;
using GGolbik.SecurityToolsApp.Core;
using GGolbik.SecurityToolsApp.Diagnostic;
using GGolbik.SecurityToolsApp.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Serilog.Events;

namespace GGolbik.SecurityToolsApp.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class LogController : ControllerBase
{
    private readonly ILogger<LogController> _logger;
    private readonly ILoggingService _logService;

    public LogController(ILogger<LogController> logger, ILoggingService logService)
    {
        _logger = logger;
        _logService = logService;
    }

    /// <summary>
    /// Returns the log configuration.
    /// </summary>
    /// <returns>The log configuration.</returns>
    /// <response code="200">The 200 (OK) status code indicates that the request has succeeded. The payload contains the application configuration.</response>
    /// <response code="500">The 500 (Internal Server Error) status code indicates that the server encountered an unexpected condition that prevented it from fulfilling the request.</response>
    [ProducesResponseType(typeof(LogConfig), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [HttpGet("config")]
    public IActionResult GetConfig()
    {
        _logger.LogTrace(MethodBase.GetCurrentMethod()?.Name);
        return Ok(new LogConfig() { Level = _logService.Level });
    }

    /// <summary>
    /// Updates the configuration.
    /// </summary>
    /// <param name="config"></param>
    /// <response code="204">The 204 (No Content) status code indicates that the request has succeeded.</response>
    /// <response code="400">The 400 (Bad Request) status code indicates that the server cannot or will not process the request due to something that is perceived to be a client error (for example, malformed request syntax, invalid request message framing, or deceptive request routing).</response>
    /// <response code="500">The 500 (Internal Server Error) status code indicates that the server encountered an unexpected condition that prevented it from fulfilling the request.</response>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [HttpPut("config")]
    public IActionResult SetConfig(LogConfig config)
    {
        _logger.LogTrace(MethodBase.GetCurrentMethod()?.Name);
        try
        {
            _logService.Level = config.Level;
            return NoContent();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to set log config.");
            throw;
        }
    }

    /// <summary>
    /// Returns the log entries.
    /// </summary>
    /// <returns>The application configuration.</returns>
    /// <response code="200">The 200 (OK) status code indicates that the request has succeeded. The payload contains the application configuration.</response>
    /// <response code="500">The 500 (Internal Server Error) status code indicates that the server encountered an unexpected condition that prevented it from fulfilling the request.</response>
    [ProducesResponseType(typeof(IList<LogEntry>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [HttpGet("entries")]
    public IActionResult GetEntries(
        [FromQuery] ulong? limit = null,
        [FromQuery] ulong? offset = null,
        [FromQuery] string? level = null,
        [FromQuery] string? sorting = null,
        [FromQuery] string? search = null,
        [FromQuery] DateTime? since = null,
        [FromQuery] DateTime? until = null
    )
    {
        _logger.LogTrace(MethodBase.GetCurrentMethod()?.Name);
        var filter = new LogFilter();
        filter.Limit = limit ?? 0;
        filter.Offset = offset ?? 0;
        if (!string.IsNullOrWhiteSpace(sorting))
        {
            if (!Enum.TryParse<SortDirection>(sorting, out var direction))
            {
                throw Errors.CreateInvalidArgumentError("sorting", "invalid");
            }
            filter.Direction = direction;
        }
        if (!string.IsNullOrWhiteSpace(search))
        {
            filter.FilterRenderedMessage = filter.FilterExceptions = filter.FilterProperties = new List<string>() { search };
        }
        if (!string.IsNullOrWhiteSpace(level))
        {
            if (!Enum.TryParse<LogLevel>(level, out var l))
            {
                throw Errors.CreateInvalidArgumentError("level", "invalid");
            }
            filter.FilterLevel = new List<LogEventLevel>();
            switch (l)
            {
                case LogLevel.Trace:
                    filter.FilterLevel?.Add(LogEventLevel.Verbose);
                    goto case LogLevel.Debug;
                case LogLevel.Debug:
                    filter.FilterLevel?.Add(LogEventLevel.Debug);
                    goto case LogLevel.Information;
                case LogLevel.None:
                case LogLevel.Information:
                    filter.FilterLevel?.Add(LogEventLevel.Information);
                    goto case LogLevel.Warning;
                case LogLevel.Warning:
                    filter.FilterLevel?.Add(LogEventLevel.Warning);
                    goto case LogLevel.Error;
                case LogLevel.Error:
                    filter.FilterLevel?.Add(LogEventLevel.Error);
                    goto case LogLevel.Critical;
                case LogLevel.Critical:
                    filter.FilterLevel?.Add(LogEventLevel.Fatal);
                    break;
            }
        }
        filter.Since = since;
        filter.Until = until;
        return Ok(_logService.GetEntries(filter));
    }

    /// <summary>
    /// Deletes all log entries
    /// </summary>
    /// <response code="204">The 204 (No Content) status code indicates that the server has successfully fulfilled the request and that there is no content to send in the response payload body.</response>
    /// <response code="400">The 400 (Bad Request) status code indicates that the server cannot or will not process the request due to something that is perceived to be a client error (for example, malformed request syntax, invalid request message framing, or deceptive request routing).</response>
    /// <response code="500">The 500 (Internal Server Error) status code indicates that the server encountered an unexpected condition that prevented it from fulfilling the request.</response>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [HttpDelete("entries")]
    public IActionResult DeleteEntries()
    {
        _logger.LogTrace(MethodBase.GetCurrentMethod()?.Name);
        _logService.DeleteEntries();
        return NoContent();
    }

}
