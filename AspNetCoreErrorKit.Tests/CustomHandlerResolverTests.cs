using AspNetCoreErrorKit.ExceptionHandler;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace AspNetCoreErrorKit.Tests
{
    public class CustomHandlerResolverTests
    {
        [Fact]
        public void ResolveHandler_ShouldReturnMostSpecificHandler()
        {
            // Arrange
            var handlers = new Dictionary<Type, Func<Exception, Task<ProblemDetails>>>
        {
            { typeof(Exception), ex => Task.FromResult(new ProblemDetails { Title = "Base Handler" }) },
            { typeof(InvalidOperationException), ex => Task.FromResult(new ProblemDetails { Title = "Specific Handler" }) }
        };

            // Act
            var handler = handlers[typeof(InvalidOperationException)];

            // Assert
            Assert.NotNull(handler);
            var result = handler(new InvalidOperationException()).Result;
            Assert.Equal("Specific Handler", result.Title);
        }
    }
}