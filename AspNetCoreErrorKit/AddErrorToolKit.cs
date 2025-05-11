using AspNetCoreErrorKit.ExceptionHandler;
using AspNetCoreErrorKit.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCoreErrorKit;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the ExceptionHandlingOptions in the service collection.
    /// </summary>
    /// <param name="services">The IServiceCollection instance.</param>
    /// <param name="configure">The action to configure ExceptionHandlingOptions.</param>
    /// <returns>The updated IServiceCollection.</returns>
    public static IServiceCollection AddExceptionHandlingOptions(
        this IServiceCollection services,
        Action<ExceptionHandlingOptions> configure)
    {
        var options = new ExceptionHandlingOptions();
        configure(options);
        services.AddSingleton(options);
        return services;
    }

    /// <summary>
    /// Register the GlobalExceptionMiddleware with the application's request pipeline.
    /// </summary>
    /// <param name="app">The IApplicationBuilder instance.</param>
    /// <returns>The updated IApplicationBuilder.</returns>
    public static IApplicationBuilder UseGlobalExceptionMiddleware(
        this IApplicationBuilder app)
    {
        var options = app.ApplicationServices.GetRequiredService<ExceptionHandlingOptions>();
        return app.UseMiddleware<ExceptionHandlingMiddleware>(options);
    }
}
