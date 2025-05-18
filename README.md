# AspNetCoreErrorKit

AspNetCoreErrorKit is a C# library that brings robust, flexible exception handling to your ASP.NET Core (.NET 8) applications. It provides global middleware and filter-based exception handling, custom error responses, async logging, and telemetry integration—all with easy setup and full extensibility.

![GitHub release (latest by date)](https://img.shields.io/github/v/release/bugzyGeek/AspNetCoreErrorKit)
![Nuget](https://img.shields.io/nuget/v/AspNetCoreErrorKit)
![License](https://img.shields.io/github/license/bugzyGeek/AspNetCoreErrorKit)

---

## Why use AspNetCoreErrorKit?

Exception handling is critical for building reliable, maintainable, and secure web APIs. AspNetCoreErrorKit lets you:

- Centralize error handling with middleware or filters
- Return consistent, customizable error responses
- Integrate async logging and telemetry
- Easily add or override handlers for specific exception types
- Use per-controller or per-action exception strategies

---

## Features

- **Global Exception Handling Middleware**: Catch and handle unhandled exceptions across your entire ASP.NET Core pipeline.
- **Exception Handling Filter**: Attribute-based, per-controller/action exception handling for fine-grained control.
- **Custom Exception Handlers**: Map exception types to custom `ProblemDetails` responses.
- **Async Logging & Telemetry**: Plug in your own async logger and telemetry tracker.
- **Easy Integration**: Extension methods for quick setup in your `Program.cs`.

---

## Version Compatibility

AspNetCoreErrorKit is compatible with:

- **.NET 8** (recommended)

---

## Installation

Install via NuGet:

```bash
dotnet add package AspNetCoreErrorKit
```

Or use the NuGet Package Manager in Visual Studio.

---

## Getting Started

### 1. Register Exception Handling Options

Add in your `Program.cs`:

```csharp
using AspNetCoreErrorKit;

builder.Services.AddExceptionHandlingOptions(options =>
{
    options.IncludeExceptionDetails = builder.Environment.IsDevelopment();
    options.CustomHandlers[typeof(InvalidOperationException)] = async (ex) =>
        new Microsoft.AspNetCore.Mvc.ProblemDetails
        {
            Title = "Invalid Operation",
            Detail = ex.Message
        };
    // Configure AsyncLogger, TelemetryTracker, DefaultHandler as needed
});
```


### 2. Add the Exception Handling Filter (Optional)

Register globally or per-controller/action:

```csharp
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ExceptionHandlingFilter>();
});
```

Or use `[ServiceFilter(typeof(ExceptionHandlingFilter))]` on controllers/actions.

### 3. Add the Exception Handling Middleware

In your pipeline:

```csharp
app.UseGlobalExceptionMiddleware(); 
app.MapControllers(); 
app.Run();
```

---

## Advanced: Custom Exception Handler Providers

Implement `ICustomExceptionHandlerProvider` on your controller for per-controller exception handler mappings.

```csharp
public class MyController : ControllerBase, ICustomExceptionHandlerProvider
{
    public IDictionary<string, IDictionary<Type, Func<Exception, Task<ProblemDetails>>>> GetCustomHandlerMappings()
    {
        return new Dictionary<string, IDictionary<Type, Func<Exception, Task<ProblemDetails>>>>
        {
            ["myKey"] = new Dictionary<Type, Func<Exception, Task<ProblemDetails>>>
            {
                [typeof(ArgumentNullException)] = async (ex) => new ProblemDetails
                {
                    Title = "Argument Null",
                    Detail = ex.Message
                }
            }
        };
    }
}

```

---

## Real-World Use Cases

- **Web APIs**: Centralize and standardize error handling in ASP.NET Core APIs.
- **Microservices**: Ensure consistent error responses and logging across services.
- **Enterprise Apps**: Integrate with telemetry and monitoring for production diagnostics.
- **Testing**: Easily mock and verify error handling in unit/integration tests.

---

## Further Information

- [Documentation](https://github.com/bugzyGeek/AspNetCoreErrorKit/wiki)
- [GitHub Repository](https://github.com/bugzyGeek/AspNetCoreErrorKit)

---

## How to contribute?

This project is open source and welcomes contributions! Please see our [CONTRIBUTING](https://github.com/bugzyGeek/AspNetCoreErrorKit/blob/main/CONTRIBUTING.md) file for guidelines.

---

## License

This project is licensed under the MIT License - see our [LICENSE](https://github.com/bugzyGeek/AspNetCoreErrorKit/blob/main/LICENSE) file for details.
