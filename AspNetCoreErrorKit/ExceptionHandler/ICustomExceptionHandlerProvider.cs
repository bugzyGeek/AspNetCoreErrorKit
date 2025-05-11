using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreErrorKit.ExceptionHandler
{
    public interface ICustomExceptionHandlerProvider
    {
        IDictionary<string, IDictionary<Type, Func<Exception, Task<ProblemDetails>>>> GetCustomHandlerMappings();
    }
}
