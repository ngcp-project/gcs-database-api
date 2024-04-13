using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using StackExchange.Redis;
using Database.Handlers;
using System.Text.Json;

namespace Database.Controllers;


[ApiController]
[Route("api/[controller]")]
public class MissionInfoController : ControllerBase
{
    private ConnectionMultiplexer conn;
    private readonly IDatabase _redis;



public MissionInfoController(){
        conn = DBConn.Instance().getConn();

        _redis = conn.GetDatabase();
        
    }

    
    [HttpGet("{missionName}")]
    public string GetMissionInfo(string missionName){
        return _redis.StringGet(missionName);

    }


}
