using Geek.Server.Core.Actors;
using Geek.Server.Core.Comps;
using Geek.Server.Core.Storage;
using MemoryPack;

namespace Geek.Server.App.Logic.Role.Pet
{

    [MemoryPackable]
    public partial class PetState : CacheState
    {

    }

    [Comp(ActorType.Role)]
    public class PetComp : StateComp<PetState>
    {
    }
}
