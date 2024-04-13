using System.Net.WebSockets;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using StackExchange.Redis;
using Database.Handlers;
using System.Text.Json;
using System.IO;


namespace Database.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MissionStagesController : ControllerBase
{
        private ConnectionMultiplexer conn;

        private readonly IDatabase _redis;
public MissionStagesController(IConnectionMultiplexer redis){

        conn = DBConn.Instance().getConn();

        _redis = conn.GetDatabase();
    }

    
    [HttpGet("GetMissionStages")]
    public async Task<string> GetMissionStages(string stageID){

        // string missionData;
        // using (var sr = new StreamReader(Request.Body)){
        //     missionData = await sr.ReadToEndAsync();
        // }

        string key = $"missionStage-{stageID}";
        var missionStageDataStr = await _redis.GetDatabase().StringGetAsync(key);
        var jsonData = JsonSerializer.Deserialize<MissionStages>(missionStageDataStr);

         var response = new
                {
                    jsonData.Key,
                    jsonData.StageName,
                    jsonData.StageStatus,
                    jsonData.VehicleKeys
                };

                
    return JsonSerializer.Serializer(response);

    }

}
