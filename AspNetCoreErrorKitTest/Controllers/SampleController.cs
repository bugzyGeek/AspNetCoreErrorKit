using AspNetCoreErrorKit.ExceptionHandler;
using AspNetCoreErrorKit.Models;
using AspNetCoreErrorKitTest.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AspNetCoreErrorKitTest.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ErrorResponseFilter]
    public class SampleController : ControllerBase
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
        [ErrorResponseFilter]
        public IActionResult MethodLevelException()
        {
            throw new Exception("Method-level exception");
        }

        [HttpGet("mixed-exception")]
        [ErrorResponseFilter(CustomErrorCodeGeneratorName = "CustomErrorCodeGenerator")]
        public IActionResult MixedException()
        {
            throw new Exception("Mixed exception");
        }

        [HttpGet("class-excption")]
        public IActionResult ClassException()
        {
            new ExceptionService().GetClassException();

            return Ok();
        }

        [HttpGet("method-excption")]
        public IActionResult MethodException()
        {
            new ExceptionService().GetMethodException();

            return Ok();
        }

        public static int CustomErrorCodeGenerator(Exception ex)
        {
            return 400;
        }
    }
}
