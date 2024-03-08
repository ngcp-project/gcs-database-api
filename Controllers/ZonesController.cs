using System.Net.Http.Headers;
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
    [HttpGet("zones/out")]
    public String getOutZones()
    {
        // return keep-out zones
        return _redis.StringGet("keepOut");
    }

    [HttpPost("zones/in")]
    public async Task postKeepIn()
    {
        using (var sr = new StreamReader(Request.Body))
        {
            string content = await sr.ReadToEndAsync();
            await _redis.StringSetAsync("keepIn", content);
        }
    }

    [HttpPost("zones/out")]
    public async Task postKeepOut()
    {
        using (var sr = new StreamReader(Request.Body))
        {
            string content = await sr.ReadToEndAsync();
            await _redis.StringSetAsync("keepOut", content);
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
