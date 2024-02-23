using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace Database.Controllers;

[ApiController]
[Route("api/EmergencyStop")]

// // Name file endpoint name + Controller
public class EmergencyStopController : ControllerBase
{
    
    // [HttpPost(Name = "PostEmergencyStop")]
    // public async Task PostEmergencyStop(){
    //     ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("local:redis:6379");
    //     IDatabase gcs = redis.GetDatabase();
    //     gcs.StringSet("EmergencyStop", "true");
    // }
    

//     [HttpGet]

//     [HttpPost]
}