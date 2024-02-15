using Microsoft.AspNetCore.Mvc;

namespace GGolbik.SecurityTools.Core;

public class SecurityToolsException : Exception
{
    private ProblemDetails _details;

    public ProblemDetails Details
    {
        get
        {
            return _details;
        }
    }

    public SecurityToolsException(string? message) : this(message, null)
    {
    }

    public SecurityToolsException(string? message, Exception? innerException) : base(message, innerException)
    {
        _details = new ProblemDetails()
        {
            Type = "Unspecified",
            Title = "Unspecified",
            Detail = message,
            Status = StatusCodes.Status500InternalServerError,
        };
    }

    public SecurityToolsException(ProblemDetails details) : this(details.Detail, null)
    {
    }

    public SecurityToolsException(ProblemDetails details, Exception? innerException) : base(details.Detail, innerException)
    {
        _details = details;
    }

}