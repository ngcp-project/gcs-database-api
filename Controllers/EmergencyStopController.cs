using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System.Text.Json;
using Database.Models;

// // Name file endpoint name + Controller
public class EmergencyStopController : ControllerBase
{
    private ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
    private readonly IDatabase gcs;

    public EmergencyStopController()
    {
        gcs = redis.GetDatabase();
    }

    [HttpPost("PostEmergencyStop")]
    public async Task PostEmergencyStop(){
        using (var sr = new StreamReader(Request.Body)){
            string vehicleData = await sr.ReadToEndAsync();
            VehicleKey? vehicleKey = JsonSerializer.Deserialize<VehicleKey>(vehicleData);
            string key = $"{vehicleKey?.Name}-status";
            await gcs.StringSetAsync(key,  "3"); //enum: 3 = emergency stop
        }
    }

    [HttpGet("test")]
    public String getTest()
    {
        return gcs.StringGet("test");
    }

    //[HttpPost("test")]
    //public void postTest()
    //{
    //    gcs.StringSet("test","tyyr!!!!!");
    //}
    

//     [HttpGet]

//     [HttpPost]
}