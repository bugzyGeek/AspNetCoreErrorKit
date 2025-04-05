using AspNetCoreErrorKit.ExceptionHandler;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreErrorKitTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GlobalHandlerController : ControllerBase
    {
        [HttpGet("global-exception")]
        public IActionResult GlobalException()
        {
            throw new Exception("Global exception");
        }

        [HttpGet("class-level-exception")]
        public IActionResult ClassLevelException()
        {
            throw new Exception("Class-level exception");
        }

        [HttpGet("method-level-exception")]
        public IActionResult MethodLevelException()
        {
            throw new Exception("Method-level exception");
        }

        [HttpGet("mixed-exception")]
        public IActionResult MixedException()
        {
            throw new Exception("Mixed exception");
        }
    }
}
