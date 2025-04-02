using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreErrorKit.Models
{
    internal class ErrorResponse
    {
        public string ErrorCode { get; set; } = null!;
        public string ReferenceId { get; set; } = null!;
        public string Message { get; set; } = null!;
        public string UserInstructions { get; set; } = null!;
    }
}
