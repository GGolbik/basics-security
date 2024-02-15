using GGolbik.SecurityTools.Core;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace GGolbik.SecurityTools.Web;

public class ErrorActionFilter : ActionFilterAttribute
{
    private readonly IWebHostEnvironment _env;

    public ErrorActionFilter(IWebHostEnvironment env)
    {
        _env = env;
    }

    public override void OnActionExecuted(ActionExecutedContext actionExecutedContext)
    {
        if (actionExecutedContext.Exception == null)
        {
            return;
        }
        string baseUri;
        try
        {
            var uri = new Uri(UriHelper.GetEncodedUrl(actionExecutedContext.HttpContext.Request));
            baseUri = String.Format("{0}{1}{2}/doc/errors/", uri.Scheme, Uri.SchemeDelimiter, uri.Authority);
        }
        catch
        {
            baseUri = "";
        }
        // create the response object.
        var details = Errors.ToProblemDetails(actionExecutedContext.Exception, baseUri, _env.IsDevelopment());
        // set response body and response HTTP status code
        actionExecutedContext.Result = new ObjectResult(details)
        {
            StatusCode = details.Status
        };
        // tell the framwork, that the execption has been handled.
        actionExecutedContext.ExceptionHandled = true;

        // log error
        if (_env.IsDevelopment())
        {
            Serilog.Log.ForContext(actionExecutedContext.Controller.GetType()).Error(actionExecutedContext.Exception, details.Detail ?? "");
        }
        else
        {
            Serilog.Log.ForContext(actionExecutedContext.Controller.GetType()).Error(details.Detail ?? "");
        }
    }
}