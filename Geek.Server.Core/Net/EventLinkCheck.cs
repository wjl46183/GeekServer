using MemoryPack;

namespace Geek.Server.Core.Net;

/// <summary>
/// 消息验证基础类型
/// </summary>
[MemoryPackable]
public partial class EventLinkCheck : BaseEvent
{
    /// <summary>
    /// 链接类型
    /// </summary>
    public string LinkType { get; set; }
    
    /// <summary>
    /// 链接用户静态ID
    /// </summary>
    public string OpenId { get; set; }
    
    /// <summary>
    /// 链接用户动态ID
    /// </summary>
    public string Token { get; set; }
    
    /// <summary>
    /// 链接签名（重连的时候确认是切换，还是重连）
    /// </summary>
    public string Sign { get; set; }
    
    /// <summary>
    /// 链接确认前的缓存session的唯一ID
    /// </summary>
    [MemoryPackIgnore]
    public long cacheSessionSerialId;
}