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
    public IActionResult GetMissionStage([FromQuery] MissionStageGET requestBody)
    {
        List<string> missingFields = new List<string>();

        EndpointReturn endpointReturn = new EndpointReturn("", "", "");
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
                endpointReturn.error = "Missing fields: " + string.Join(", ", missingFields);
                return BadRequest(endpointReturn.ToString());
            }
        }
        string result = gcs.StringGet("missionStage-" + requestBody.stageId);
        endpointReturn.data = result;
        return Ok(endpointReturn.ToString());
    }


    [HttpPost("MissionStage")]
    public async Task<IActionResult> SetMissionStage([FromBody] MissionStage requestBody)
    {
        List<string> missingFields = new List<string>();

        EndpointReturn endpointReturn = new EndpointReturn("", "", "");
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
                endpointReturn.error = "Missing fields: " + string.Join(", ", missingFields);
                return BadRequest(endpointReturn.ToString());
            }

        }

        // Enum Validation
        if (!Enum.IsDefined(typeof(Stage_Enum), requestBody.stageStatus))
        {
            endpointReturn.error = "Invalid Stage_Enum";
            return BadRequest(endpointReturn.ToString());
        }

        await gcs.StringSetAsync("missionStage-" + requestBody.stageId, requestBody.ToString());
        endpointReturn.message = "Posted MissionStage";
        return Ok(endpointReturn.ToString());
    }
}