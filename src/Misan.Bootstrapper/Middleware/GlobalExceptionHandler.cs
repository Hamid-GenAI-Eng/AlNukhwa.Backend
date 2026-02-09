using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Misan.Shared.Kernel.Exceptions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Misan.Bootstrapper.Middleware;

public class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var problemDetails = new ProblemDetails
        {
            Instance = httpContext.Request.Path
        };

        if (exception is ValidationException validationException)
        {
            problemDetails.Title = "Validation Failure";
            problemDetails.Status = StatusCodes.Status400BadRequest;
            problemDetails.Detail = "One or more validation errors occurred.";
            problemDetails.Extensions["errors"] = validationException.Errors;
        }
        else if (exception is BadRequestException badRequestException)
        {
            problemDetails.Title = badRequestException.Title;
            problemDetails.Status = StatusCodes.Status400BadRequest;
            problemDetails.Detail = badRequestException.Message;
        }
        else if (exception is NotFoundException notFoundException)
        {
            problemDetails.Title = notFoundException.Title;
            problemDetails.Status = StatusCodes.Status404NotFound;
            problemDetails.Detail = notFoundException.Message;
        }
        else
        {
            // Log the actual exception
            Serilog.Log.Error(exception, "Unhandled exception occurred while processing request {RequestPath}", httpContext.Request.Path);

            problemDetails.Title = "Server Error";
            problemDetails.Status = StatusCodes.Status500InternalServerError;
            problemDetails.Detail = "An internal server error has occurred.";
        }

        httpContext.Response.StatusCode = problemDetails.Status.Value;

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
