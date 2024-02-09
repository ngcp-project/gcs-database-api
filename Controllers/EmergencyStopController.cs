using Microsoft.AspNetCore.Mvc;

namespace Database.Controllers;

[ApiController]
[Route("api/EmergencyStop")]

// // Name file endpoint name + Controller
public class EmergencyStopController : ControllerBase
{
    
    [HttpPost(Name = "PostEmergencyStop")]
    public async Task PostEmergencyStop(){

    }
    

//     [HttpGet]

//     [HttpPost]
}