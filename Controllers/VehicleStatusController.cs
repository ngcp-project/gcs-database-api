using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System.Text.Json;
using Database.Models;

public class VehicleStatusController : ControllerBase
{
    private ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
    private readonly IDatabase gcs;

    public VehicleStatusController()
    {
        gcs = redis.GetDatabase();
    }

/*
--------------Posts and Gets----------------
Body format for all posts and gets in this file:
{
    "Key": "[VehicleName]"
}
*/

    [HttpPost("SetStatusInUse")]
    public async Task SetStatusInUse(){
        using (var sr = new StreamReader(Request.Body)){
            string vehicleData = await sr.ReadToEndAsync();
            VehicleKey? vehicleKey = JsonSerializer.Deserialize<VehicleKey>(vehicleData);
            string key = $"{vehicleKey?.Key}-status"; //Status Key Format: [VehicleName]-status
            await gcs.StringSetAsync(key,  "1"); //enum: 1 = In Use
        }
    }

    [HttpPost("SetStatusStandby")]
    public async Task SetStatusStandby(){
        using (var sr = new StreamReader(Request.Body)){
            string vehicleData = await sr.ReadToEndAsync();
            VehicleKey? vehicleKey = JsonSerializer.Deserialize<VehicleKey>(vehicleData);
            string key = $"{vehicleKey?.Key}-status";
            await gcs.StringSetAsync(key,  "2"); //enum: 2 = Stand By
        }
    }

    [HttpPost("EmergencyStop")]
    public async Task EmergencyStop(){
        using (var sr = new StreamReader(Request.Body)){
            string vehicleData = await sr.ReadToEndAsync();
            VehicleKey? vehicleKey = JsonSerializer.Deserialize<VehicleKey>(vehicleData);
            string key = $"{vehicleKey?.Key}-status";
            await gcs.StringSetAsync(key,  "3"); //enum: 3 = Emergency Stopped
        }
    }

    [HttpGet("GetVehicleStatus")]
    public async Task<string> GetVehicleStatus(){
        int num = 0; //Default of 0, database value will be read into this variable
        using (var sr = new StreamReader(Request.Body)){
            string vehicleData = await sr.ReadToEndAsync();
            VehicleKey? vehicleKey = JsonSerializer.Deserialize<VehicleKey>(vehicleData);
            string key = $"{vehicleKey?.Key}-status";
            num = (int) await gcs.StringGetAsync(key);
        }
        string output; //output string, will be returned. 
        //Switch block to conert int value in database to string value for enum. 
        switch (num){
            case 1: 
                output = "In Use";
                break;
            case 2:
                output = "Standby";
                break;
            case 3:
                output = "Emergency Stopped";
                break;
            default:
                output = "Invalid";
                break;
        }
        return output;
    }
}