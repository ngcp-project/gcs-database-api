using System.Net.Http.Headers;
using System.Reflection;
using Database.Models;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace Database.Controllers;
public class VehicleDataController : ControllerBase
{
    private ConnectionMultiplexer conn;
    private readonly IDatabase _redis;

    public VehicleDataController()
    {
        conn = DBConn.Instance().getConn();
        _redis = conn.GetDatabase();
    }
    [HttpGet("vehicleData")]
    public String getVehicleData()
    {
        // return vehicle data
        return _redis.StringGet("vehicleData");
    }

    [HttpPost("vehicleData")]
    public async Task<IActionResult> postVehicleData([FromBody] VehicleData requestBody)
    {
        List<string> missingFields = new List<string>();

        Type type = typeof(VehicleData); 
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
        // Iterates through every property in the model and checks if it is null or default value

        // If any field is missing, return a bad request
        using (var sr = new StreamReader(Request.Body))
        {
            string content = await sr.ReadToEndAsync();
            await _redis.StringSetAsync("vehicleData", content);
        }
        await _redis.StringSetAsync("vehicleData",requestBody.ToString()); // Replace "example" with the respective database key
        return Ok("Posted VehicleData successfully!");
    }

}
