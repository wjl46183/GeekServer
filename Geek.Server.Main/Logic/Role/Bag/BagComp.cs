using Geek.Server.Core.Actors;
using Geek.Server.Core.Comps;
using Geek.Server.Core.Storage;
using MemoryPack;

namespace Geek.Server.Main.Logic.Role.Bag;

[MemoryPackable]
public partial class BagState : CacheState
{
    
    public Dictionary<int, long> ItemMap = new Dictionary<int, long>();
}

[Comp(ActorType.Role)]
public class BagComp : StateComp<BagState>
{
}