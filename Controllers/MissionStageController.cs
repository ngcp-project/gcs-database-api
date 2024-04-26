using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using Database.Models;
using System.Reflection;
using System.Text.Json;

public class MissionStageController : ControllerBase
{
    private ConnectionMultiplexer conn;
    private readonly IDatabase gcs;

    public MissionStageController()
    {
        conn = DBConn.Instance().getConn();
        gcs = conn.GetDatabase();
    }


    [HttpGet("MissionStage")]
    public IActionResult GetMissionStage([FromBody] MissionStageGET requestBody)
    {
        List<string> missingFields = new List<string>();

        Type type = typeof(MissionStageGET);
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
        string result = gcs.StringGet("missionStage-" + requestBody.stageId);
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
        if (!Enum.IsDefined(typeof(Stage_Enum), requestBody.stageStatus))
        {
            return BadRequest("Invalid Stage_Enum");
        }

        // if (gcs.StringGet("missionStage-" + requestBody.stageId).IsNullOrEmpty)
        // {
        //     await gcs.StringSetAsync("missionStage-" + requestBody.stageId, requestBody.ToString());
        //     return Ok("Posted MissionStage");
        // }
        // // Initializes mission stage entry if it does not exist

        // MissionStage currentMissionStageEntry = JsonSerializer.Deserialize<MissionStage>(gcs.StringGet("missionStage-" + requestBody.stageId));



        // await gcs.StringAppendAsync("missionStage-" + requestBody.stageId, requestBody.ToString());
        await gcs.StringSetAsync("missionStage-" + requestBody.stageId, requestBody.ToString());
        return Ok("Posted MissionStage");
    }
}