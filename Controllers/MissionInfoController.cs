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
    private readonly IDatabase gcs;



public MissionInfoController(){
        conn = DBConn.Instance().getConn();

        gcs = conn.GetDatabase();
        
    }

    
    [HttpGet("GetMissionInfo")]
    public IActionResult getExample()
    {
        return gcs.StringGet("{missionName}");
    }


}
