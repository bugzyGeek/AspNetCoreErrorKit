using AspNetCoreErrorKit.ExceptionHandler;
using AspNetCoreErrorKit.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace AspNetCoreErrorKit.Tests
{
    public class GlobalExceptionMiddlewareTests
    {
        [Fact]
        public async Task Middleware_ShouldCallDefaultHandler_WhenNoCustomHandlerExists()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<ExceptionHandlingMiddleware>>();
            var options = new ExceptionHandlingOptions
            {
                DefaultHandler = async (context, ex, opts) =>
                {
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    await context.Response.WriteAsync("Default handler executed.");
                }
            };

            var middleware = new ExceptionHandlingMiddleware(async (context) =>
            {
                throw new InvalidOperationException("Test exception");
            }, loggerMock.Object, options);

            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var response = new StreamReader(context.Response.Body).ReadToEnd();
            Assert.Equal("Default handler executed.", response);
        }
    }
}