using Geek.Server.Core.Actors;
using Geek.Server.Core.Comps;
using Geek.Server.Core.Storage;
using MemoryPack;

namespace Geek.Server.Storage.Role.Bag;

[MemoryPackable]
[SaveState(ActorType.Role)]
public partial class BagState : BaseState
{
    public Dictionary<int, long> ItemMap = new Dictionary<int, long>();
}