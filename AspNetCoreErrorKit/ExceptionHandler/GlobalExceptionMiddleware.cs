using System.Text.Json;
using AspNetCoreErrorKit.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AspNetCoreErrorKit.ExceptionHandler;

internal class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    private readonly IOptions<ErrorHandlingOptions> _options;

    public GlobalExceptionMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionMiddleware> logger,
        IOptions<ErrorHandlingOptions> options)
    {
        _next = next;
        _logger = logger;
        _options = options;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            var endpoint = context.GetEndpoint();
            if (endpoint?.Metadata.GetMetadata<ErrorResponseFilterAttribute>() != null)
            {
                throw;
            }

            if (_options.Value.LogExceptions && (_options.Value.ShouldLogException?.Invoke(ex) ?? true))
            {
                _logger.LogError(ex, "Unhandled Exception caught in GlobalExceptionMiddleware");
            }

            int errorCode = _options.Value.CustomErrorCodeGenerator?.Invoke(ex) ?? 500;

            context.Response.StatusCode = errorCode;
            context.Response.ContentType = _options.Value.UseProblemDetails ? "application/problem+json" : "application/json";

            var response = _options.Value.UseProblemDetails
                ? JsonSerializer.Serialize(new ProblemDetails
                {
                    Type = $"https://httpstatuses.com/{errorCode}",
                    Title = errorCode >= 500 ? "Internal Server Error" : "A problem occurred",
                    Status = errorCode,
                    Detail = _options.Value.EnableDetailedErrors ? ex.Message : _options.Value.DefaultErrorMessage,
                    Instance = context.Request.Path
                })
                : JsonSerializer.Serialize(new ErrorResponse
                {
                    ReferenceId = Guid.NewGuid().ToString(),
                    ErrorCode = errorCode.ToString(),
                    UserInstructions = _options.Value.DefaultErrorMessage,
                    Message = _options.Value.EnableDetailedErrors ? ex.Message : _options.Value.DefaultErrorMessage
                });

            await context.Response.WriteAsync(response);
        }
    }
}
