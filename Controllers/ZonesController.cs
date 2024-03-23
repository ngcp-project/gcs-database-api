// using System.Net.Http.Headers;
// using System.Text;
using System.Text.Json;
// using System.Text.Json.Nodes;
// using System.Text.Json.Serialization;
using Database.Models;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;


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
    public async Task<IActionResult> postKeepIn()
    {
        using (var sr = new StreamReader(Request.Body))
        {
            string content = await sr.ReadToEndAsync();
            string contentCopy = content; // copy to validate

            // validate fields
            try
            {
                Zone json = JsonSerializer.Deserialize<Zone>(contentCopy);
                // json.keepIn = true;

                Console.WriteLine("Check passed!");
                await _redis.StringSetAsync("keepIn", content);
                Console.WriteLine("keepIn set.");
                return Ok();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                return BadRequest();
            }

        }
    }

    [HttpPost("zones/out")]
    public async Task<IActionResult> postKeepOut()
    {
        using (var sr = new StreamReader(Request.Body))
        {
            string content = await sr.ReadToEndAsync();
            string contentCopy = content;

            // validate fields
            try
            {
                Console.WriteLine("Testing input...");
                Zone json = JsonSerializer.Deserialize<Zone>(contentCopy);
                // json.keepIn = false;

                Console.WriteLine("Check passed!");
                await _redis.StringSetAsync("keepOut", content);
                Console.WriteLine("keepOut set.");
                return Ok();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                return BadRequest();
            }
        }
    }

    // [HttpDelete("zones/in")]
    // public async Task delKeepIn(string key)
    // {
    //     return key;
    // }

    // [HttpDelete("zones/out")]
    // public async Task delKeepOut(string key)
    // {
    //     return key;
    // }

}
