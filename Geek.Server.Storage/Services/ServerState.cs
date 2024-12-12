using Geek.Server.Core.Actors;
using Geek.Server.Core.Comps;
using Geek.Server.Core.Storage;
using MemoryPack;

namespace Geek.Server.Storage;

[MemoryPackable]
[SaveState(ActorType.PhysicServer)]
public partial class ServerState : BaseState
{
    /// <summary>
    /// 世界等级
    /// </summary>
    public int WorldLevel { get; set; } = 1;
}