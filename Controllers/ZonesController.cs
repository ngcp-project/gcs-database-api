using System.Reflection;
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
    public String getInZones()
    {
        // return keep-in zones
        return _redis.StringGet("keepIn");
    }

    [HttpGet("zones/out")]
    public String getOutZones()
    {
        // return keep-out zones
        return _redis.StringGet("keepOut");
    }

    [HttpPost("zones/in")]
    public async Task<IActionResult> PostKeepIn([FromBody] Zone requestBody)
    {
        List<string> missingFields = new List<string>();

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

            if (value?.Equals(defaultValue) == true || value == null)
            {
                missingFields.Add(property.Name);
            }

            if (missingFields.Count > 0)
            {
                return BadRequest("Missing fields: " + string.Join(", ", missingFields));
            }
            // If any field is missing, return a bad request
        }
        requestBody.keepIn = true;
        if (requestBody.zoneShapeType == ShapeType.Unknown)
        {
            return BadRequest("Invalid shape type");
        }


        // await _redis.StringSetAsync("keepIn", requestBody.ToString()); // Replace "example" with the respective database key
        await _redis.StringAppendAsync("keepIn", requestBody.ToString());
        return Ok("Posted keepIn zone successfully.");
    } // end postKeepIn

    [HttpPost("zones/out")]
    public async Task<IActionResult> postKeepOut([FromBody] Zone requestBody)
    {
        List<string> missingFields = new List<string>();

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

            if (value?.Equals(defaultValue) == true || value == null)
            {
                missingFields.Add(property.Name);
            }

            // If any field is missing, return a bad request
            if (missingFields.Count > 0)
            {
                return BadRequest("Missing fields: " + string.Join(", ", missingFields));
            }
            
        }

        // validate shape type
        requestBody.keepIn = false;
        if (requestBody.zoneShapeType == ShapeType.Unknown)
        {
            return BadRequest("Invalid shape type");
        }


        // await _redis.StringSetAsync("keepOut", requestBody.ToString()); // Replace "example" with the respective database key
        await _redis.StringAppendAsync("keepOut", requestBody.ToString());
        return Ok("Posted keepOut zone successfully.");
    } // end postKeepOut

    // [HttpDelete("zones/in")]
    // public async Task delKeepIn([FromBody] Zone requestBody)
    // {
    //     return key;
    // }

    // [HttpDelete("zones/out")]
    // public async Task delKeepOut([FromBody] Zone requestBody)
    // {
    //     return key;
    // }

}
