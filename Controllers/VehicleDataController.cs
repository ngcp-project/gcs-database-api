using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;


public class VehicleDataController : ControllerBase
{
    private ConnectionMultiplexer conn;
    private readonly IDatabase _redis;

    public VehicleDataController()
    {
        conn = DBConn.Instance().getConn();
        _redis = conn.GetDatabase();
    }
    [HttpGet("vehicledata/get")]
    public String getVehicleData()
    {
        // return vehicle data
        return _redis.StringGet("vehicleData");
    }

    [HttpPost("vehicledata/post")]
    public async Task postVehicleData()
    {
        using (var sr = new StreamReader(Request.Body))
        {
            var content = await sr.ReadToEndAsync();
            await _redis.StringSetAsync("vehicleData", content);
        }
    }
}
