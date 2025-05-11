using AspNetCoreErrorKit.ExceptionHandler;
using AspNetCoreErrorKit.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace AspNetCoreErrorKit.Tests
{
    public class ExceptionHandlingFilterTests
    {
        [Fact]
        public async Task Filter_ShouldCallCustomHandler_WhenHandlerExists()
        {
            // Arrange
            var loggerMock = new Mock<ILogger>();
            var options = new ExceptionHandlingOptions();
            options.CustomHandlers[typeof(InvalidOperationException)] = async (ex) =>
            {
                return new Microsoft.AspNetCore.Mvc.ProblemDetails
                {
                    Title = "Custom Handler",
                    Detail = ex.Message
                };
            };

            var filter = new ExceptionHandlingFilter(loggerMock.Object, options, "testKey");

            var httpContext = new DefaultHttpContext();
            var actionContext = new ActionContext(httpContext, new Microsoft.AspNetCore.Routing.RouteData(), new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor());
            var context = new ExceptionContext(actionContext, new List<IFilterMetadata>())
            {
                Exception = new InvalidOperationException("Test exception")
            };

            // Act
            await filter.OnExceptionAsync(context);

            // Assert
            Assert.True(context.ExceptionHandled);
        }
    }
}