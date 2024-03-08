using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;


public sealed class DBConn
{
    private static DBConn _instance = null;
    private static readonly object _lock = new object();
    private DBConn() {}
    private ConnectionMultiplexer conn { get; set;}

    public static DBConn Instance()
    {
        if (_instance == null)
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = new DBConn();
                    _instance.conn = ConnectionMultiplexer.Connect("localhost");
                }
            }
        }
        return _instance;
    }

    public ConnectionMultiplexer getConn() {
        return _instance.conn;
    }
}