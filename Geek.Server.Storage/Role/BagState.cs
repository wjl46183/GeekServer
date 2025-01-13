using Geek.Server.Core.Actors;
using Geek.Server.Core.Comps;
using Geek.Server.Core.Storage;
using MemoryPack;

namespace Geek.Server.Storage;

[MemoryPackable]
[SaveState(ActorType.Role)]
public partial class BagState : BaseState
{
    public Dictionary<int, long> ItemMap = new Dictionary<int, long>();
    public Dictionary<int, long> ItemMap21 = new Dictionary<int, long>();
}