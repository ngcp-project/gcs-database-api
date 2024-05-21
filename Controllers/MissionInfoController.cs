using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using Database.Models;
using System.Reflection;
using System.Text.Json;
using System.Linq;


namespace Database.Controllers;
public class MissionInfoController : ControllerBase
{
    private ConnectionMultiplexer conn;
    private readonly IDatabase gcs;

    public MissionInfoController()
    {
        conn = DBConn.Instance().getConn();
        gcs = conn.GetDatabase();
    }

    [HttpGet("MissionInfo")]
    public IActionResult GetMissionInfo([FromQuery] MissionInfoGET requestBody)
    {

        List<string> missingFields = new List<string>();

        EndpointReturn endpointReturn = new EndpointReturn("", "", "");
        Type type = typeof(MissionInfoGET);
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
        string result = gcs.StringGet(requestBody.missionName).ToString();
        endpointReturn.data = result;
        return Ok(endpointReturn.ToString());
    }


    [HttpPost("MissionInfo")]
    public async Task<IActionResult> SetMissionInfo([FromBody] MissionInfoPOST requestBody)
    {
        List<string> missingFields = new List<string>();

        EndpointReturn endpointReturn = new EndpointReturn("", "", "");
        Type type = typeof(MissionInfoPOST);
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

        List<MissionStage> stages = new List<MissionStage>();
        MissionStage initialStage = new MissionStage(requestBody.stageName, requestBody.vehicleKeys);
        stages.Add(initialStage);

        MissionInfo missionInfo = new MissionInfo
        {
            missionName = requestBody.missionName,
            // currentStageId = requestBody.stageName,
            stages = stages.ToArray()
        };
        // Initializes new MissionInfo object with a MissionStage attached to it


        await gcs.StringSetAsync(requestBody.missionName, missionInfo.ToString());
        endpointReturn.message = "Posted MissionInfo";
        return Ok(endpointReturn.ToString());
    }

    [HttpPost("CurrentStage")]
    public async Task<IActionResult> SetCurrentStage([FromBody] CurrentStagePOST requestBody)
    {
        List<string> missingFields = new List<string>();

        EndpointReturn endpointReturn = new EndpointReturn("", "", "");
        Type type = typeof(CurrentStagePOST);
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

        // Pull MissionInfo from database and update currentStageId
        try
        {
            MissionInfo missionInfo = JsonSerializer.Deserialize<MissionInfo>(gcs.StringGet(requestBody.missionName));
            foreach (MissionStage stage in missionInfo.stages)
            {
                if (stage.stageName == requestBody.currentStage)
                {
                    missionInfo.currentStageId = requestBody.currentStage;
                    await gcs.StringSetAsync(requestBody.missionName, missionInfo.ToString());

                    endpointReturn.message = "Posted CurrentStage";
                    return Ok(endpointReturn.ToString());
                }
            }
        }
        catch (Exception e)
        {
            endpointReturn.error = e.Message;
            return BadRequest(endpointReturn.ToString());
        }

        endpointReturn.error = "MissionStage does not exist";
        return BadRequest(endpointReturn.ToString());
    }
}
