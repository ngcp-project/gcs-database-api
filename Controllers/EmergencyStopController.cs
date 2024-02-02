using Microsoft.AspNetCore.Mvc;

namespace Database.Controllers;

[ApiController]
[Route("api/EmergencyStop")]

// // Name file endpoint name + Controller
public class EmergencyStopController : ControllerBase
{
    [HttpGet(Name = "GetEmergencyStop")]
    public async Task GetEmergencyStopAsync(){
        
    }

    [HttpPost(Name = "PutEmergencyStop")]
    public async Tast PutEmergencyStop(){

    }
    

//     [HttpGet]

//     [HttpPost]
}