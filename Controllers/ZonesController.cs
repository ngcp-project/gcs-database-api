using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using StackExchange.Redis;

public class ZonesController : ControllerBase
{
    private ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
    private readonly IDatabase _redis;

    public ZonesController()
    {
        _redis = redis.GetDatabase();
    }

    [HttpGet("zones/{keepIn}")]
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
    public async Task postKeepIn()
    {
        using (var sr = new StreamReader(Request.Body))
        {
            var content = await sr.ReadToEndAsync();
            await _redis.StringSetAsync("keepIn", content);
        }
    }

    [HttpPost("zones/out")]
    public async Task postKeepOut()
    {
        using (var sr = new StreamReader(Request.Body))
        {
            var content = await sr.ReadToEndAsync();
            await _redis.StringSetAsync("keepOut", content);
        }
    }


}
