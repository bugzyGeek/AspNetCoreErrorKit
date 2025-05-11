using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AspNetCoreErrorKit.Models;

public class ExceptionHandlingOptions
{
    /// <summary>
    /// Determines if detailed exception information should be included in the error responses.
    /// </summary>
    public bool IncludeExceptionDetails { get; set; } = false;

    /// <summary>
    /// Dictionary mapping an exception type to a custom handling delegate.
    /// </summary>
    public IDictionary<Type, Func<Exception, Task<ProblemDetails>>> CustomHandlers { get; } =
        new Dictionary<Type, Func<Exception, Task<ProblemDetails>>>();

    /// <summary>
    /// An asynchronous logger delegate. In a real-world scenario, this might be tied to an async sink (like Serilog’s async logging).
    /// </summary>
    public Func<ILogger, Exception, Task> AsyncLogger { get; set; } =
        async (logger, ex) =>
        {
            // Wrap the logging call in Task.Run to simulate async behavior.
            await Task.Run(() => logger.LogError(ex, "Unhandled exception occurred."));
        };

    /// <summary>
    /// An optional delegate to report exception details to an external telemetry system.
    /// </summary>
    public Func<HttpContext, Exception, Task>? TelemetryTracker { get; set; } = null;

    /// <summary>
    /// The default exception handling delegate. It now receives the options to check IncludeExceptionDetails.
    /// </summary>
    public Func<HttpContext, Exception, ExceptionHandlingOptions, Task> DefaultHandler { get; set; } =
        async (context, ex, options) =>
        {
            int statusCode = StatusCodes.Status500InternalServerError;
            context.Response.StatusCode = statusCode;
            // Use detailed info or a generic message based on config.
            var detail = options.IncludeExceptionDetails ? ex.ToString() : "An unexpected error occurred.";
            var problem = new ProblemDetails
            {
                Title = "Server Error",
                Status = statusCode,
                Detail = detail
            };

            await context.Response.WriteAsJsonAsync(problem);
        };
}

