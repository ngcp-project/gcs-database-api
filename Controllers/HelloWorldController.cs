using Microsoft.AspNetCore.Mvc;

namespace Database.Controllers;


// Template
// Name file endpoint name + Controller
public class HelloWorldController : ControllerBase
{
    [HttpGet("api/hello-world")]
    public IActionResult HelloWorld()
    {
        return Ok("Hello World!!");
    }
}