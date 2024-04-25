using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System.Text.Json;
using Database.Models;
using System.Reflection;

public class MissionStageController : ControllerBase
{
    private ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
    private readonly IDatabase gcs;

    public MissionStageController()
    {
        gcs = redis.GetDatabase();
    }


    [HttpGet("GetMissionStage")]
    public IActionResult GetMissionStage()
    {
        return gcs.StringGet("missionStage-{stageId}");
    }


    [HttpPost("MissionStage")]
    public async Task<IActionResult> SetMissionStage([FromBody] MissionStage requestBody)
    {
        List<string> missingFields = new List<string>();

        Type type = typeof(MissionStage);
        PropertyInfo[] properties = type.GetProperties();

        foreach (System.Reflection.PropertyInfo property in properties)
        {
            var value = property.GetValue(requestBody, null);
            object defaultValue = null;
            if (property.PropertyType == typeof(string))
            {
                defaultValue = null;
            }

            else if (property.PropertyType.IsValueType)
            {
                defaultValue = Activator.CreateInstance(property.PropertyType);
            }

            if (value?.Equals(defaultValue) == true || value == null)
            {
                missingFields.Add(property.Name);
            }
            if (missingFields.Count > 0)
            {
                return BadRequest("Missing fields: " + string.Join(", ", missingFields));
            }
            
        }

        // Enum Validation
        if(!Enum.IsDefined(typeof(Stage_Enum), requestBody.StageStatus)){
            return BadRequest("Invalid Stage_Enum");
        }

        await _redis.StringAppendAsync("missionStage-{stageId}", requestBody.ToString());
        return Ok("Posted MissionStage");
    } 
}