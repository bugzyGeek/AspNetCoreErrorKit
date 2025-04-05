using AspNetCoreErrorKit.ExceptionHandler;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreErrorKitTest.Controllers;

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

    public static int CustomErrorCodeGenerator(Exception ex)
    {
        return 400;
    }
}
