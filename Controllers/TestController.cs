using Microsoft.AspNetCore.Mvc;
using aspnetenrichedexceptionlogger.Middleware;

namespace aspnetenrichedexceptionlogger.Controllers;

[ApiController]
[Route("[controller]")]
public class TestController : ControllerBase
{
    private readonly ILogger<TestController> _logger;

    public TestController(ILogger<TestController> logger)
    {
        _logger = logger;
    }

    [HttpGet("test")]
    public string GetTest()
    {
        HttpContext.EnrichUnhandledExceptionLogger("SomeId", "1234");
        HttpContext.EnrichUnhandledExceptionLogger("SomeOtherId", "ASDLKFJAS2342");
        HttpContext.EnrichUnhandledExceptionLogger("Something with spaces", "Hello!");
        throw new Exception("alskdjfasfd");
        _logger.LogInformation($"{nameof(GetTest)} called");
        return "blah";
    }
}
