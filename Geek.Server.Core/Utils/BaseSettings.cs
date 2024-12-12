using Geek.Server.Core.Utils;
using Newtonsoft.Json;

namespace Geek.Server.Core.Utils;
public class BaseSetting
{
    public virtual bool IsLocal(int serverId)
    {
        return serverId == ServerId;
    }

    public DateTime LauchTime { get; set; }

    public volatile bool AppRunning = false;

    public ServerType ServerType { get; set; }

    #region from config
    public bool IsDebug { get; init; }

    public int ServerId { get; init; }

    public string ServerName { get; init; }

    public string LocalIp { get; init; }

    public string HttpCode { get; init; }

    public string HttpUrl { get; init; }

    public int HttpPort { get; init; }

    public int TcpPort { get; init; }

    public int GrpcPort { get; init; }

    public string WebSocketUrl { get; init; }

    public string MongoUrl { get; init; }

    public string MongoDBName { get; init; } 
    public string Language { get; init; }

    public string DataCenter { get; init; }

    public string CenterUrl { get; init; }

    public int SDKType { get; set; } 
    #endregion
}