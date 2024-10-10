using Microsoft.AspNetCore.Mvc;

namespace GGolbik.SecurityTools.Core;

public static class Errors
{
    private static SecurityToolsException CreateError(Exception? innerException, string type, string title, int status, string format, params object?[] args)
    {
        var details = new ProblemDetails()
        {
            Type = type,
            Title = title,
            Detail = String.Format(format, args),
            Status = status
        };
        return new SecurityToolsException(details, innerException);
    }

    private static SecurityToolsException CreateError(string type, string title, int status, string format, params object?[] args)
    {
        return CreateError(null, type, title, status, format, args);
    }

    /// <summary>
    /// Creates a new UnspecifiedError error.
    /// </summary>
    /// <remarks>
    /// The error is not specified.
    /// </remarks>
    /// <returns>The error.</returns>
    public static SecurityToolsException CreateUnspecifiedError(string? message = null)
    {
        return new SecurityToolsException(message);
    }

    public static SecurityToolsException CreateUnspecifiedError(Exception e, string? message = null)
    {
        return new SecurityToolsException(message, e);
    }

    /// <summary>
    /// Creates a new UnspecifiedError error.
    /// </summary>
    /// <remarks>
    /// The error is not specified.
    /// </remarks>
    /// <returns>The error.</returns>
    public static SecurityToolsException CreateUnspecifiedError(string format, params object?[] args)
    {
        return new SecurityToolsException(String.Format(format, args));
    }

    public static SecurityToolsException CreateUnspecifiedError(Exception innerException, string format, params object?[] args)
    {
        return new SecurityToolsException(String.Format(format, args), innerException);
    }

    /// <summary>
    /// Creates a new InvalidArgument error.
    /// </summary>
    /// <remarks>
    /// The argument is unknown or the value invalid.
    /// </remarks>
    /// <returns>The error.</returns>
    public static SecurityToolsException CreateInvalidArgumentError(string argumentName, string? format, params object?[] args)
    {
        if(String.IsNullOrWhiteSpace(format))
        {
            return Errors.CreateError("InvalidArgument", "Invalid Argument", StatusCodes.Status400BadRequest, "Invalid Argument: {0}.", argumentName);
        }
        return Errors.CreateError("InvalidArgument", "Invalid Argument", StatusCodes.Status400BadRequest, "Invalid Argument: {0}. {1}", argumentName, String.Format(format, args));
    }

    public static SecurityToolsException CreateInvalidArgumentError(Exception e, string argumentName, string? format, params object?[] args)
    {
        if(String.IsNullOrWhiteSpace(format))
        {
            return Errors.CreateError(e, "InvalidArgument", "Invalid Argument", StatusCodes.Status400BadRequest, "Invalid Argument: {0}.", argumentName);
        }
        return Errors.CreateError(e, "InvalidArgument", "Invalid Argument", StatusCodes.Status400BadRequest, "Invalid Argument: {0}. {1}", argumentName, String.Format(format, args));
    }

    public static ProblemDetails ToProblemDetails(Exception exception, string baseUri, bool isDevelopment)
    {
        var result = new ProblemDetails();
        if (exception is SecurityToolsException)
        {
            result.Type = (exception as SecurityToolsException)?.Details.Type;
            result.Title = (exception as SecurityToolsException)?.Details.Title;
            result.Detail = (exception as SecurityToolsException)?.Details.Detail;
            result.Status = (exception as SecurityToolsException)?.Details.Status;
        }
        else
        {
            result.Type = "Unexpected";
            result.Title = isDevelopment ? "Unexpected" : exception.Message;
            result.Detail = isDevelopment ? exception.ToString() : "Unexpected error.";
            result.Status = StatusCodes.Status500InternalServerError;
        }
        if (isDevelopment && exception.InnerException != null)
        {
            var detail = $"{result.Detail} {exception.InnerException.ToString()}";
            result.Detail = detail;
        }
        result.Type = $"{baseUri}{result.Type}";
        return result;
    }
}