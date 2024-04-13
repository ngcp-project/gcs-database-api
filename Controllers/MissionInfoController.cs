using Microsoft.AspNetCore.Mvc;
using StackExchange.redis;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Database.Controllers;


[ApiController]
[Route("api/[controller]")]
public class MissionInfo : ControllerBase
{
   private ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
   private readonly IDatabase _redis;



public MissionInfo(){
        conn = DBConn.Instance().getConn()
        _redis = redis.GetDatabase();
    }

    
    [HttpGet("{missionName}")]
    public string GetMissionInfo(string missionName){
        return _redis.StringGet(missionName);

    }


}
