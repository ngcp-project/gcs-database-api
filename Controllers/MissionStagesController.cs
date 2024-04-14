using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System.Text.Json;
using Database.Models;
using System.Reflection;

public class MissionStagesController : ControllerBase
{
    private ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
    private readonly IDatabase gcs;

    public MissionStagesController()
    {
        gcs = redis.GetDatabase();
    }


    [HttpGet("GetMissionStages")]
    public async Task<string> GetMissionStages(string stageID){

        // string missionData;
        // using (var sr = new StreamReader(Request.Body)){
        //     missionData = await sr.ReadToEndAsync();
        // }

        string key = $"missionStage-{stageID}";
        var missionStageDataStr = await gcs.StringGetAsync(key);
        var jsonData = JsonSerializer.Deserialize<MissionStages>(missionStageDataStr);

         var response = new
                {
                    jsonData.key,
                    jsonData.stageName,
                    jsonData.stageStatus,
                    // jsonData.VehicleKeys
                };
    return JsonSerializer.Serialize(response);
    }

}