using Microsoft.AspNetCore.Mvc;

namespace Database.Controllers;

[ApiController]
[Route("api/[controller]")]

// Template
// Name file endpoint name + Controller
public class ExampleController : ControllerBase
{
    [HttpGet(Name = "GetWeatherForecast")]

    [HttpPost(Name = "GetWeatherForecast")]

    [HttpGet]

    [HttpPost]
}