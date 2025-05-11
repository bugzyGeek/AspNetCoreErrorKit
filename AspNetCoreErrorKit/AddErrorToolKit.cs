using AspNetCoreErrorKit.ExceptionHandler;
using AspNetCoreErrorKit.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCoreErrorKit;

public static class ServiceCollectionExtensions
{

    /// <summary>
    /// Register the GlobalExceptionMiddleware with the application's request pipeline.
    /// </summary>
    /// <param name="app">The IApplicationBuilder instance.</param>
    /// <returns>The updated IApplicationBuilder.</returns>
    public static IApplicationBuilder UseGlobalExceptionMiddleware(
        this IApplicationBuilder builder,
        Action<ExceptionHandlingOptions> configure)
    {
        var options = new ExceptionHandlingOptions();
        configure(options);
        builder.ApplicationServices.GetRequiredService<IServiceCollection>()
            .AddSingleton(options);
        return builder.UseMiddleware<ExceptionHandlingMiddleware>(options);
    }
}
