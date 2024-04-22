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


    [HttpGet("GetMissionStage")]
    public IActionResult getMissionStage()
    {
        return gcs.StringGet("missionStage-{stageId}");
    }
}