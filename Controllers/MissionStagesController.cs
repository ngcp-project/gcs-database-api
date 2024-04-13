using Microsoft.AspNetCore.Mvc;
using StackExchange.redis;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Database.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MissionStagesController : ControllerBase
{
   private ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
   private readonly IDatabase _redis;



public MissionStagesController(){
        conn = DBConn.Instance().getConn()
        _redis = redis.GetDatabase();
    }

    
    [HttpGet("GetMissionStages")]
    public async Task<string> GetMissionStages(string stageID){

        // string missionData;
        // using (var sr = new StreamReader(Request.Body)){
        //     missionData = await sr.ReadToEndAsync();
        // }

        string key = $"missionStage-{stageID}";
        var missionStageDataStr = await _redis.GetDatabase().StringGetAsync(key);
        var jsonData = JsonSerializer.Deserialize<VehicleStages>(redisValue);


        var responseObj = new
            {
                jsonData.CurrentStageId,
                jsonData.StageName,
                jsonData.stageStatus
                jsonData.vehicleKeys = jsonObj.vehicleKeys
            };

    }
    return JsonSerializer.Serializer(jsonObj)

}
