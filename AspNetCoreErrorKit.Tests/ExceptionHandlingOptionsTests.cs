using AspNetCoreErrorKit.Models;
using Xunit;

namespace AspNetCoreErrorKit.Tests
{
    public class ExceptionHandlingOptionsTests
    {
        [Fact]
        public void DefaultOptions_ShouldHaveExpectedDefaults()
        {
            // Arrange & Act
            var options = new ExceptionHandlingOptions();

            // Assert
            Assert.False(options.IncludeExceptionDetails);
            Assert.NotNull(options.CustomHandlers);
            Assert.NotNull(options.AsyncLogger);
            Assert.NotNull(options.DefaultHandler);
        }
    }
}