using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreErrorKit.ExceptionHandler
{
    public static class ServiceLocator
    {
        public static IServiceProvider? Instance { get; set; }
    }
}
