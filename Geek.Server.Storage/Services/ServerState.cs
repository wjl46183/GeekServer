using Geek.Server.Core.Storage;
using MemoryPack;

namespace Server.Storage.Services;

[MemoryPackable]
public partial class ServerState : CacheState
{
    /// <summary>
    /// 世界等级
    /// </summary>
    public int WorldLevel { get; set; } = 1;
}