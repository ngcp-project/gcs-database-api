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
    public IActionResult getInZones()
    {
        EndpointReturn endpointReturn = new EndpointReturn("", "", "");
        // return keep-in zones
        if (_redis.StringGet("keepIn").IsNullOrEmpty)
        {
            endpointReturn.error = "No keep-in zones found.";
            return BadRequest(endpointReturn.ToString());
        }

        endpointReturn.data = _redis.StringGet("keepIn").ToString();
        return Ok(endpointReturn.ToString());
    }

    [HttpGet("zones/out")]
    public IActionResult getOutZones()
    {
        EndpointReturn endpointReturn = new EndpointReturn("", "", "");
        // return keep-out zones
        if (_redis.StringGet("keepOut").IsNullOrEmpty)
        {
            endpointReturn.error = "No keep-out zones found.";
            return BadRequest(endpointReturn.ToString());
        }

        endpointReturn.data = _redis.StringGet("keepOut").ToString();
        return Ok(endpointReturn.ToString());
    }

    [HttpPost("zones/in")]
    public async Task<IActionResult> PostKeepIn([FromBody] Zone requestBody)
    {
        List<string> missingFields = new List<string>();

        EndpointReturn endpointReturn = new EndpointReturn("", "", "");
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

        Console.WriteLine(requestBody.ToString());
        if (_redis.StringGet("keepIn").IsNullOrEmpty)
        {
            await _redis.StringSetAsync("keepIn", requestBody.ToString());
            endpointReturn.message = "Posted keepIn zone successfully.";
            return Ok(endpointReturn.ToString());
        }
        // Initializes the array to have the first element as the first zone

        await _redis.StringAppendAsync("keepIn", "|" + requestBody.ToString());
        endpointReturn.message = "Posted keepIn zone successfully.";
        return Ok(endpointReturn.ToString());
    } // end postKeepIn

    [HttpPost("zones/out")]
    public async Task<IActionResult> postKeepOut([FromBody] Zone requestBody)
    {
        List<string> missingFields = new List<string>();

        EndpointReturn endpointReturn = new EndpointReturn("", "", "");
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


        if (_redis.StringGet("keepOut").IsNullOrEmpty)
        {
            await _redis.StringSetAsync("keepOut", requestBody.ToString());
            endpointReturn.message = "Posted keepOut zone successfully.";
            return Ok(endpointReturn.ToString());
        }
        // Initializes the array to have the first element as the first zone

        await _redis.StringAppendAsync("keepOut", "|" + requestBody.ToString());
        endpointReturn.message = "Posted keepOut zone successfully.";
        return Ok(endpointReturn.ToString());
    } // end postKeepOut

    [HttpDelete("zones/in")]
     public IActionResult clearKeepIn()
    {
        EndpointReturn endpointReturn = new EndpointReturn("", "", "");
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
        EndpointReturn endpointReturn = new EndpointReturn("", "", "");
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
