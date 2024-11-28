using Geek.Server.Core.Actors;
using Geek.Server.Core.Comps;
using Geek.Server.Core.Storage;
using MemoryPack;

namespace Geek.Server.Storage.Services;

[MemoryPackable]
[DynamicState<ServerState>]
public partial class DynamicServerState : IDynamicState
{
    /// <summary>
    /// 存放在此处的数据不会回存到数据库
    /// </summary>
    public HashSet<long> OnlineSet = new();
}