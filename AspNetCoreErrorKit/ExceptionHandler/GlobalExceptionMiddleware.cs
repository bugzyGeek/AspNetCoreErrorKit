using System.Text.Json;
using AspNetCoreErrorKit.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AspNetCoreErrorKit.ExceptionHandler;

internal class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly ExceptionHandlingOptions _options;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        ExceptionHandlingOptions options)
    {
        _next = next;
        _logger = logger;
        _options = options;
    }

    // Helper method to determine the inheritance "distance" between the registered Exception type and the thrown exception.
    private static int GetInheritanceDistance(Type baseType, Type type)
    {
        int distance = 0;
        while (type != null && type != baseType)
        {
            type = type.BaseType;
            distance++;
        }
        return (type == null ? int.MaxValue : distance);
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            // Asynchronously log the exception.
            await _options.AsyncLogger(_logger, ex);

            // Integrate telemetry features if a telemetry tracker has been provided.
            if (_options.TelemetryTracker != null)
            {
                await _options.TelemetryTracker(context, ex);
            }

            // Hierarchical handling: choose the most specific custom handler if available.
            var exceptionType = ex.GetType();
            var matchingHandler = _options.CustomHandlers
                .Where(handler => handler.Key.IsAssignableFrom(exceptionType))
                .OrderBy(handler => GetInheritanceDistance(handler.Key, exceptionType))
                .FirstOrDefault();

            if (matchingHandler.Key != null)
            {
                var response = await matchingHandler.Value(ex);
                response.Detail = _options.IncludeExceptionDetails ? ex.ToString() : response.Detail;
                await context.Response.WriteAsJsonAsync(response);
            }
            else
            {
                // Use the default handler while passing the options so it can decide on detail level.
                await _options.DefaultHandler(context, ex, _options);
            }
        }
    }
}

