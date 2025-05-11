using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AspNetCoreErrorKit.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AspNetCoreErrorKit.ExceptionHandler
{
    public class ExceptionHandlingFilter : IAsyncExceptionFilter, IFilterFactory
    {
        private readonly ILogger _logger;
        private readonly ExceptionHandlingOptions _options;
        private readonly string _handlerKey;


        public ExceptionHandlingFilter(string handlerKey)
        {
            _handlerKey = handlerKey;
        }

        public ExceptionHandlingFilter(ILogger logger, ExceptionHandlingOptions options, string handlerKey)
        {
            _logger = logger;
            _options = options;
            _handlerKey = handlerKey;
        }

        public async Task OnExceptionAsync(ExceptionContext context)
        {
            await _options.AsyncLogger(_logger, context.Exception);

            if (_options.TelemetryTracker != null)
            {
                await _options.TelemetryTracker(context.HttpContext, context.Exception);
            }

            if (context.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor && typeof(ICustomExceptionHandlerProvider).IsAssignableFrom(controllerActionDescriptor.ControllerTypeInfo))
            {

                var handlerProvider = (ICustomExceptionHandlerProvider)context.HttpContext.RequestServices.GetService(controllerActionDescriptor.ControllerTypeInfo);
                if (handlerProvider != null)
                {
                    var handlerMappings = handlerProvider.GetCustomHandlerMappings();
                    if (handlerMappings.TryGetValue(_handlerKey, out var exceptionHandlers))
                    {
                        var exceptionType = context.Exception.GetType();
                        var matchingHandler = _options.CustomHandlers
                        .Where(handler => handler.Key.IsAssignableFrom(exceptionType))
                        .OrderBy(handler => ExceptionHandlingMiddleware.GetInheritanceDistance(handler.Key, exceptionType))
                        .FirstOrDefault();

                        if (matchingHandler.Key != null)
                        {
                            var response = await matchingHandler.Value(context.Exception);
                            context.ExceptionHandled = true;
                            await context.HttpContext.Response.WriteAsJsonAsync(response);
                            return;
                        }
                    }
                }
            }

            await _options.DefaultHandler(context.HttpContext, context.Exception, _options);
            context.ExceptionHandled = true;
        }

        // Implement IFilterFactory to resolve dependencies dynamically
        public bool IsReusable => false;

        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            var logger = serviceProvider.GetRequiredService<ILogger<ExceptionHandlingFilter>>();
            var options = serviceProvider.GetRequiredService<ExceptionHandlingOptions>();
            return new ExceptionHandlingFilter(logger, options, _handlerKey);
        }
    }

}
