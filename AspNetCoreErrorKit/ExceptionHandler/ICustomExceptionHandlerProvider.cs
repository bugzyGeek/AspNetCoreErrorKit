using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreErrorKit.ExceptionHandler
{
    public interface ICustomExceptionHandlerProvider
    {
        IDictionary<string, IDictionary<Type, Func<Exception, Task<ProblemDetails>>>> GetCustomHandlerMappings();
    }
}
