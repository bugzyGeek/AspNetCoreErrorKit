
# AspNetCoreErrorKit

AspNetCoreErrorKit is a robust and extensible library for handling errors and exceptions in ASP.NET Core applications. It provides middleware and attributes for centralized and customizable error handling, including support for structured responses and RFC 7807-compliant `ProblemDetails`.

## Features

- **Global Exception Middleware**: Catch and handle all unhandled exceptions globally in the request pipeline.
- **Action-Level Error Filter**: Fine-grained control over error responses at the controller or action level using the `ErrorResponseFilterAttribute`.
- **Customizable Responses**:
  - Standard error responses (`ErrorResponse`).
  - RFC 7807-compliant `ProblemDetails`.
- **Configuration Options**:
  - Toggle detailed error messages.
  - Customize default error messages.
  - Delegate-based error code generation and conditional logging.
- **Extensibility**: Dynamically resolve custom methods for error code generation and exception logging conditions.

## Installation

Add the NuGet package (coming soon) to your project:

```bash
dotnet add package AspNetCoreErrorKit
```

## Usage

### 1. Configure Services

In your `Startup.cs` or `Program.cs`, register AspNetCoreErrorKit services in the DI container:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddControllers();
    services.AddAspNetCoreErrorKit(
        Configuration,
        customErrorCodeGenerator: ex => ex is ArgumentException ? 400 : 500,
        shouldLogException: ex => !(ex is ArgumentException)
    );
}
```

### 2. Enable Global Middleware

Register the middleware in your application's request pipeline:

```csharp
public void Configure(IApplicationBuilder app)
{
    app.UseGlobalExceptionMiddleware();
    app.UseRouting();
    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
    });
}
```

### 3. Use the Filter Attribute

Decorate controllers or actions with the `ErrorResponseFilterAttribute` to enable action-specific exception handling:

```csharp
[ApiController]
[Route("api/[controller]")]
[ErrorResponseFilter(CustomErrorCodeGeneratorName = "CustomErrorCodeGenerator")]
public class SampleController : ControllerBase
{
    [HttpGet("test-error")]
    public IActionResult TestError()
    {
        throw new Exception("Test error");
    }

    public static int CustomErrorCodeGenerator(Exception ex)
    {
        return 400;
    }
}
```

### 4. Configuration Options

Use the `ErrorHandlingOptions` class to customize behavior. Configure options in `appsettings.json` under the desired section name (default: `ErrorHandlingOptions`):

```json
"ErrorHandlingOptions": {
  "DefaultErrorMessage": "Oops! Something went wrong.",
  "EnableDetailedErrors": false,
  "LogExceptions": true,
  "UseProblemDetails": true
}
```

## Response Formats

### Standard Error Response

When `UseProblemDetails` is set to `false`, the error response follows this format:
```json
{
  "referenceId": "12345",
  "errorCode": "500",
  "message": "An unexpected error occurred. Please try again later.",
  "userInstructions": "An unexpected error occurred. Please try again later."
}
```

### RFC 7807-Compliant ProblemDetails

When `UseProblemDetails` is set to `true`, the error response follows this format:
```json
{
  "type": "https://httpstatuses.com/500",
  "title": "Internal Server Error",
  "status": 500,
  "detail": "An unexpected error occurred.",
  "instance": "/api/sample/test-error"
}
```

## Best Practices

- Use global middleware for handling unhandled exceptions.
- Apply `ErrorResponseFilterAttribute` for action-specific overrides.
- Ensure proper configuration of the `ErrorHandlingOptions` section.
- Test custom error code generators and logging logic extensively.
- Avoid leaking sensitive information in production by disabling detailed errors.

## Contributions

Contributions are welcome! Feel free to open issues or submit pull requests.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.