using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreErrorKit.Models
{
    public class ErrorHandlingOptions
    {
        public string DefaultErrorMessage { get; set; } = "An unexpected error occurred. Please try again later.";
        public bool EnableDetailedErrors { get; set; } = false;
        public bool LogExceptions { get; set; } = true;
        public bool UseProblemDetails { get; set; } = false; // New flag for RFC 7807 support

        public Func<Exception, int>? CustomErrorCodeGenerator { get; set; }
        public Func<Exception, bool>? ShouldLogException { get; set; }
    }
}
