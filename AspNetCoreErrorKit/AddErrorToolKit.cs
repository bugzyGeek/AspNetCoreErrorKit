using AspNetCoreErrorKit.ExceptionHandler;
using AspNetCoreErrorKit.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCoreErrorKit;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the AspNetCoreErrorKit services and configuration.
    /// </summary>
    /// <param name="services">The IServiceCollection instance.</param>
    /// <param name="configuration">The application's configuration.</param>
    /// <param name="configurationSection">The configuration section name.</param>
    /// <param name="customErrorCodeGenerator">Optional custom error code generator.</param>
    /// <param name="shouldLogException">Optional delegate to determine if an exception should be logged.</param>
    /// <returns>The updated IServiceCollection.</returns>
    public static IServiceCollection AddAspNetCoreErrorKit(
        this IServiceCollection services,
        IConfiguration configuration,
        string configurationSection = nameof(ErrorHandlingOptions),
        Func<Exception, int>? customErrorCodeGenerator = null,
        Func<Exception, bool>? shouldLogException = null)
    {
        // Bind the configuration section for our error handling options.
        services.Configure<ErrorHandlingOptions>(configuration.GetSection(configurationSection));

        // Configure additional options
        services.PostConfigure<ErrorHandlingOptions>(options =>
        {
            options.CustomErrorCodeGenerator = customErrorCodeGenerator;
            options.ShouldLogException = shouldLogException;
        });

        return services;
    }

    /// <summary>
    /// Register the GlobalExceptionMiddleware with the application's request pipeline.
    /// </summary>
    /// <param name="app">The IApplicationBuilder instance.</param>
    /// <returns>The updated IApplicationBuilder.</returns>
    public static IApplicationBuilder UseGlobalExceptionMiddleware(this IApplicationBuilder app)
    {
        ServiceLocator.Instance = app.ApplicationServices;
        return app.UseMiddleware<GlobalExceptionMiddleware>();
    }
}
