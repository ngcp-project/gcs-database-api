using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System.Text.Json;
using Database.Models;
using System.Reflection;


namespace Database.Controllers;
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
        string result = gcs.StringGet("missionName");
        return Ok(result);

    }


    [HttpPost("MissionInfo")]
    public async Task<IActionResult> MissionInfo([FromBody] MissionInfo requestBody)
    {
        List<string> missingFields = new List<string>();

        Type type = typeof(MissionInfo);
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


        await gcs.StringAppendAsync("missionName", requestBody.ToString());
        return Ok("Posted MissionInfo");
    } 
    


}
