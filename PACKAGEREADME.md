# AspNetCoreErrorKit

A flexible, modern exception handling toolkit for ASP.NET Core (.NET 8). Provides global middleware and filter-based exception handling, custom error responses, async logging, and telemetry integration. Easily plug into your pipeline for robust error management in your web APIs and MVC applications.

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

```cssharp
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

## Version Compatibility

AspNetCoreErrorKit is compatible with:

- **.NET 8** (recommended)

---

## Further Information

- [Documentation](https://github.com/bugzyGeek/AspNetCoreErrorKit/wiki)
- [GitHub Repository](https://github.com/bugzyGeek/AspNetCoreErrorKit)

---

## License

MIT
