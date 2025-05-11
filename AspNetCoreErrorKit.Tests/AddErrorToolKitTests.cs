using AspNetCoreErrorKit;
using AspNetCoreErrorKit.Models;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace AspNetCoreErrorKit.Tests
{
    public class AddErrorToolKitTests
    {
        [Fact]
        public void AddExceptionHandlingOptions_ShouldRegisterOptions()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            services.AddExceptionHandlingOptions(options =>
            {
                options.IncludeExceptionDetails = true;
            });

            var provider = services.BuildServiceProvider();
            var options = provider.GetRequiredService<ExceptionHandlingOptions>();

            // Assert
            Assert.NotNull(options);
            Assert.True(options.IncludeExceptionDetails);
        }
    }
}