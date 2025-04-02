using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AspNetCoreErrorKit.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AspNetCoreErrorKit.ExceptionHandler
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ErrorResponseFilterAttribute : ExceptionFilterAttribute
    {
        private readonly ILogger<ErrorResponseFilterAttribute> _logger;
        private readonly IOptions<ErrorHandlingOptions> _options;

        public ErrorResponseFilterAttribute(ILogger<ErrorResponseFilterAttribute> logger, IOptions<ErrorHandlingOptions> options)
        {
            _logger = logger;
            _options = options;
        }

        public override void OnException(ExceptionContext context)
        {
            var ex = context.Exception;

            if (_options.Value.LogExceptions && (_options.Value.ShouldLogException?.Invoke(ex) ?? true))
            {
                _logger.LogError(ex, "Unhandled Exception caught in ErrorResponseFilterAttribute");
            }

            int errorCode = _options.Value.CustomErrorCodeGenerator?.Invoke(ex) ?? 500;

            // If RFC 7807 is enabled, return a ProblemDetails response.
            if (_options.Value.UseProblemDetails)
            {
                var problemDetails = new ProblemDetails
                {
                    Type = "https://httpstatuses.com/" + errorCode,
                    Title = (errorCode >= 500)
                        ? "Internal Server Error"
                        : "A problem occurred",
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
                // Otherwise, return a custom error payload.
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

            // Optionally set ExceptionHandled to true so that it short-circuits further processing.
            context.ExceptionHandled = true;
        }
    }
}
