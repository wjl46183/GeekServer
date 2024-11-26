using Geek.Server.Core.Actors;
using Geek.Server.Core.Comps;
using Geek.Server.Core.Storage;
using MemoryPack;
using Server.Storage.Role.Bag;

namespace Geek.Server.Main.Logic.Role.Bag;

[Comp(ActorType.Role)]
public class BagComp : StateComp<BagState>
{
}