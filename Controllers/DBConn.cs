using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;


public sealed class DBConn
{
    private static DBConn _instance = null;
    private static readonly object _lock = new object();
    private DBConn() {}
    private ConnectionMultiplexer conn { get; set;}

/**
 * <summary>
 *    This construction only makes a DB connection if one doesn't exist yet.
 * </summary>
 */
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

/**
 * <summary>
 *    This function returns the current connection.
 * </summary>
 */
    public ConnectionMultiplexer getConn() {
        return _instance.conn;
    }
}