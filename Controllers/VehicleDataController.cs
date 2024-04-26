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
    public IActionResult getVehicleData([FromBody] VehicleKey requestBody)
    {
        List<string> missingFields = new List<string>();

        EndpointReturn endpointReturn = new EndpointReturn("", "", "");
        Type type = typeof(VehicleKey);
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
        // return vehicle data
        Console.WriteLine(requestBody.Key + "-geo");
        endpointReturn.data = _redis.StringGet(requestBody.Key + "-geo").ToString();
        return Ok(endpointReturn.ToString());

    }

    [HttpPost("vehicleData")]
    public async Task<IActionResult> postVehicleData([FromBody] VehicleData requestBody)
    {
        List<string> missingFields = new List<string>();

        EndpointReturn endpointReturn = new EndpointReturn("", "", "");
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
                endpointReturn.error = "Missing fields: " + string.Join(", ", missingFields);
                return BadRequest(endpointReturn.ToString());
            }

        }
        await _redis.StringSetAsync(requestBody.vehicleName + "-geo", requestBody.ToString());
        endpointReturn.data = "Posted VehicleData Successfully!";
        return Ok(endpointReturn.ToString());
    }

}
