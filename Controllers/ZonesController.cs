using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Database.Models;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace Database.Controllers;


public class ZonesController : ControllerBase
{
    private ConnectionMultiplexer conn;
    private readonly IDatabase _redis;

    public ZonesController()
    {
        conn = DBConn.Instance().getConn();
        _redis = conn.GetDatabase();
    }

    [HttpGet("zones/in")]
    public IActionResult getInZones()
    {
        EndpointReturn<Object> endpointReturn = new EndpointReturn<Object>("", "", null);
        // return keep-in zones
        if (_redis.StringGet("keepIn").IsNullOrEmpty)
        {
            endpointReturn.error = "No keep-in zones found.";
            return BadRequest(endpointReturn.ToString());
        }
        var temp = _redis.StringGet("keepIn").ToString();
        var result = JsonSerializer.Deserialize<List<Zone>>(temp);
        endpointReturn.data = result;
        
        return Ok(endpointReturn.ToString());
    }

    [HttpGet("zones/out")]
    public IActionResult getOutZones()
    {
        EndpointReturn<Object> endpointReturn = new EndpointReturn<Object>("", "", null);
        // return keep-out zones
        if (_redis.StringGet("keepOut").IsNullOrEmpty)
        {
            endpointReturn.error = "No keep-out zones found.";
            return BadRequest(endpointReturn.ToString());
        }
        var temp =  _redis.StringGet("keepOut").ToString();
        var ans = JsonSerializer.Deserialize<List<Zone>>(temp);
        endpointReturn.data = ans;
        // endpointReturn.data = _redis.StringGet("keepOut").ToString();
        return Ok(endpointReturn);
    }

    [HttpPost("zones/in")]
    public async Task<IActionResult> PostKeepIn([FromBody] Zone requestBody)
    {
        List<string> missingFields = new List<string>();

        EndpointReturn<Object> endpointReturn = new EndpointReturn<Object>("", "", null);
        Type type = typeof(Zone);
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

            if (value == null)
            {
                missingFields.Add(property.Name);
            }

            if (missingFields.Count > 0)
            {
                endpointReturn.error = "Missing fields: " + string.Join(", ", missingFields);
                return BadRequest(endpointReturn.ToString());
            }
            // If any field is missing, return a bad request
        }
        requestBody.keepIn = true;

        Console.WriteLine(requestBody);
        string existingZones = await _redis.StringGetAsync("keepIn");
        List<Zone> zones = string.IsNullOrEmpty(existingZones) ? new List<Zone>() : JsonSerializer.Deserialize<List<Zone>>(existingZones);
        zones.Add(requestBody);
        await _redis.StringSetAsync("keepIn",JsonSerializer.Serialize(zones));
        // if (_redis.StringGet("keepIn").IsNullOrEmpty)
        // {
        //     await _redis.StringSetAsync("keepIn", requestBody.ToString());
        //     endpointReturn.message = "Posted keepIn zone successfully.";
        //     return Ok(endpointReturn.ToString());
        // }
        // // Initializes the array to have the first element as the first zone
        // //Could be 1 error
        // await _redis.StringAppendAsync("keepIn", "|" + requestBody.ToString());
        // endpointReturn.message = "Posted keepIn zone successfully.";
        return Ok(endpointReturn.ToString());
    } // end postKeepIn

    [HttpPost("zones/out")]
    public async Task<IActionResult> postKeepOut([FromBody] Zone requestBody)
    {
        List<string> missingFields = new List<string>();

        EndpointReturn<Object> endpointReturn = new EndpointReturn<Object>("", "", null);
        Type type = typeof(Zone);
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

            if (value == null)
            {
                missingFields.Add(property.Name);
            }

            // If any field is missing, return a bad request
            if (missingFields.Count > 0)
            {
                endpointReturn.error = "Missing fields: " + string.Join(", ", missingFields);
                return BadRequest(endpointReturn.ToString());
            }

        }

        Console.WriteLine(requestBody);
        string existingZones = await _redis.StringGetAsync("keepOut");
        List<Zone> zones = string.IsNullOrEmpty(existingZones) ? new List<Zone>() : JsonSerializer.Deserialize<List<Zone>>(existingZones);
        zones.Add(requestBody);
        await _redis.StringSetAsync("keepOut",JsonSerializer.Serialize(zones));


        // if (_redis.StringGet("keepOut").IsNullOrEmpty)
        // {
        //     await _redis.StringSetAsync("keepOut", requestBody.ToString());
        //     endpointReturn.message = "Posted keepOut zone successfully.";
        //     return Ok(endpointReturn.ToString());
        // }
        // // Initializes the array to have the first element as the first zone

        // await _redis.StringAppendAsync("keepOut", "|" + requestBody.ToString());
        // endpointReturn.message = "Posted keepOut zone successfully.";
        return Ok(endpointReturn.ToString());
    } // end postKeepOut

    [HttpDelete("zones/in")]
     public IActionResult clearKeepIn()
    {
        EndpointReturn<Object> endpointReturn = new EndpointReturn<Object>("", "", null);
        // check keep-in zones exist
        if (_redis.StringGet("keepIn").IsNullOrEmpty)
        {
            endpointReturn.error = "No keep-in zones found.";
            return BadRequest(endpointReturn.ToString());
        }

        _redis.KeyDelete("keepIn");
        endpointReturn.message = "Cleared keep-in zones successfully.";
        return Ok(endpointReturn.ToString());
    }

     [HttpDelete("zones/out")]
     public IActionResult clearKeepOut()
    {
        EndpointReturn<Object> endpointReturn = new EndpointReturn<Object>("", "", null);
        // check keep-in zones exist
        if (_redis.StringGet("keepOut").IsNullOrEmpty)
        {
            endpointReturn.error = "No keep-out zones found.";
            return BadRequest(endpointReturn.ToString());
        }

        _redis.KeyDelete("keepOut");
        endpointReturn.message = "Cleared keep-out zones successfully.";
        return Ok(endpointReturn.ToString());
    }

}
