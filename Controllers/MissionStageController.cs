using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System.Text.Json;
using Database.Models;
using System.Reflection;

public class MissionStageController : ControllerBase
{
    private ConnectionMultiplexer conn = ConnectionMultiplexer.Connect("localhost");
    private readonly IDatabase gcs;

    public MissionStageController()
    {
        conn = DBConn.Instance().getConn();
        gcs = conn.GetDatabase();
    }


    [HttpGet("GetMissionStage")]
    public IActionResult GetMissionStage()
    {
        string result = gcs.StringGet("missionStage-{stageId}");
        return Ok(result);
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
        if(!Enum.IsDefined(typeof(Stage_Enum), requestBody.stageStatus)){
            return BadRequest("Invalid Stage_Enum");
        }

        await gcs.StringAppendAsync("missionStage-{stageId}", requestBody.ToString());
        return Ok("Posted MissionStage");
    } 
}