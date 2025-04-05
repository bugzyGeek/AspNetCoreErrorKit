using AspNetCoreErrorKit.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AspNetCoreErrorKit.ExceptionHandler;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class ErrorResponseFilterAttribute : ExceptionFilterAttribute
{
    private readonly ILogger<ErrorResponseFilterAttribute> _logger;
    private readonly IOptions<ErrorHandlingOptions> _options;

    public string? CustomErrorCodeGeneratorName { get; set; }
    public string? ShouldLogExceptionName { get; set; }

    public ErrorResponseFilterAttribute()
    {
        _logger = ServiceLocator.Instance?.GetRequiredService<ILogger<ErrorResponseFilterAttribute>>()
                  ?? throw new InvalidOperationException("Logger not found");
        _options = ServiceLocator.Instance?.GetRequiredService<IOptions<ErrorHandlingOptions>>()
                   ?? throw new InvalidOperationException("Options not found");
    }

    public override void OnException(ExceptionContext context)
    {
        var ex = context.Exception;

        var customErrorCodeGenerator = GetCustomErrorCodeGenerator(context);
        var shouldLogException = GetShouldLogException(context);

        bool shouldLog = shouldLogException?.Invoke(ex) ?? _options.Value.ShouldLogException?.Invoke(ex) ?? true;
        if (_options.Value.LogExceptions && shouldLog)
        {
            _logger.LogError(ex, "Unhandled Exception caught in ErrorResponseFilterAttribute");
        }

        int errorCode = customErrorCodeGenerator?.Invoke(ex) ?? _options.Value.CustomErrorCodeGenerator?.Invoke(ex) ?? 500;

        if (_options.Value.UseProblemDetails)
        {
            var problemDetails = new ProblemDetails
            {
                Type = "https://httpstatuses.com/" + errorCode,
                Title = (errorCode >= 500) ? "Internal Server Error" : "A problem occurred",
                Status = errorCode,
                Detail = _options.Value.EnableDetailedErrors ? ex.Message : _options.Value.DefaultErrorMessage,
                Instance = context.HttpContext.Request.Path
            };

            context.Result = new ObjectResult(problemDetails)
            {
                StatusCode = errorCode
            };
        }
        else
        {
            var errorResponse = new ErrorResponse
            {
                ReferenceId = Guid.NewGuid().ToString(),
                ErrorCode = errorCode.ToString(),
                UserInstructions = _options.Value.DefaultErrorMessage,
                Message = _options.Value.EnableDetailedErrors ? ex.Message : _options.Value.DefaultErrorMessage
            };

            context.Result = new ObjectResult(errorResponse)
            {
                StatusCode = errorCode
            };
        }

        context.ExceptionHandled = true;
    }

    private Func<Exception, int>? GetCustomErrorCodeGenerator(ExceptionContext context)
    {
        if (CustomErrorCodeGeneratorName == null) return null;

        var controllerActionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
        if (controllerActionDescriptor == null) return null;

        var controllerType = controllerActionDescriptor.ControllerTypeInfo.AsType();
        if (controllerType == null) return null;

        var method = controllerType.GetMethod(CustomErrorCodeGeneratorName,
            System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);

        if (method == null) return null;

        return (Func<Exception, int>)Delegate.CreateDelegate(typeof(Func<Exception, int>), method);



    }

    private Func<Exception, bool>? GetShouldLogException(ExceptionContext context)
    {
        if (ShouldLogExceptionName == null) return null;

        var controllerActionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
        if (controllerActionDescriptor == null) return null;

        var controllerType = controllerActionDescriptor.ControllerTypeInfo.AsType();
        if (controllerType == null) return null;

        var method = controllerType.GetMethod(ShouldLogExceptionName, System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);

        if (method == null) return null;

        return (Func<Exception, bool>)Delegate.CreateDelegate(typeof(Func<Exception, bool>), method);
    }
}
